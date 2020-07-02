module DocumentNameParserTests

open NUnit.Framework
open FsUnit
open ADP

[<Test>]
let ``File names shall be parsed properly`` () =
    DocumentNameParser.parse "Ololoev-report.pdf" 
    |> should equal (Choice<Document, string>.Choice1Of2 { 
        FileName = "Ololoev-report.pdf"; 
        Kind = Text; 
        Authors = ["Ololoev"] })

[<Test>]
let ``All document kinds shall be parsed`` () =
    DocumentNameParser.parse "Ololoev-slides.pdf" 
    |> should equal (Choice<Document, string>.Choice1Of2 { 
        FileName = "Ololoev-slides.pdf"; 
        Kind = Slides; 
        Authors = ["Ololoev"] })

    DocumentNameParser.parse "Ololoev-advisor-review.pdf" 
    |> should equal (Choice<Document, string>.Choice1Of2 { 
        FileName = "Ololoev-advisor-review.pdf"; 
        Kind = AdvisorReview; 
        Authors = ["Ololoev"] })

    DocumentNameParser.parse "Ololoev-reviewer-review.pdf" 
    |> should equal (Choice<Document, string>.Choice1Of2 { 
        FileName = "Ololoev-reviewer-review.pdf"; 
        Kind = ReviewerReview; 
        Authors = ["Ololoev"] })

    DocumentNameParser.parse "Ololoev-consultant-review.pdf" 
    |> should equal (Choice<Document, string>.Choice1Of2 { 
        FileName = "Ololoev-consultant-review.pdf"; 
        Kind = ConsultantReview; 
        Authors = ["Ololoev"] })

    DocumentNameParser.parse "Ololoev-advisor-consultant-review.pdf" 
    |> should equal (Choice<Document, string>.Choice1Of2 { 
        FileName = "Ololoev-advisor-consultant-review.pdf"; 
        Kind = AdvisorConsultantReview; 
        Authors = ["Ololoev"] })

    DocumentNameParser.parse "Ololoev-review.pdf" 
    |> should equal (Choice<Document, string>.Choice1Of2 { 
        FileName = "Ololoev-review.pdf"; 
        Kind = AdvisorReview; 
        Authors = ["Ololoev"] })

[<Test>]
let ``Incorrect file names shall be correctly declined`` () =
    DocumentNameParser.parse "index.html" 
    |> should equal (Choice<Document, string>.Choice2Of2 "index.html")

[<Test>]
let ``Works with multiple authors shall be parsed correctly`` () =
    DocumentNameParser.parse "Kizhnerov-Sokolvyak-Tuchina-slides.pdf" 
    |> should equal (Choice<Document, string>.Choice1Of2 { 
        FileName = "Kizhnerov-Sokolvyak-Tuchina-slides.pdf"; 
        Kind = Slides; 
        Authors = ["Kizhnerov"; "Sokolvyak"; "Tuchina"] })