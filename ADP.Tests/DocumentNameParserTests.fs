module DocumentNameParserTests

open NUnit.Framework
open FsUnit
open ADP

[<Test>]
let ``File names shall be parsed properly`` () =
    DocumentNameParser.parse "444-Ololoev-report.pdf" 
    |> should equal <| Some { 
        FileName = "444-Ololoev-report.pdf"; 
        Group = "444"; 
        Kind = Text; 
        Authors = ["Ololoev"] }

[<Test>]
let ``All document kinds shall be parsed`` () =
    DocumentNameParser.parse "444-Ololoev-slides.pdf" 
    |> should equal <| Some { 
        FileName = "444-Ololoev-slides.pdf"; 
        Group = "444"; 
        Kind = Slides; 
        Authors = ["Ololoev"] }

    DocumentNameParser.parse "444-Ololoev-advisor-review.pdf" 
    |> should equal <| Some { 
        FileName = "444-Ololoev-advisor-review.pdf"; 
        Group = "444"; 
        Kind = AdvisorReview; 
        Authors = ["Ololoev"] }

    DocumentNameParser.parse "444-Ololoev-reviewer-review.pdf" 
    |> should equal <| Some { 
        FileName = "444-Ololoev-reviewer-review.pdf"; 
        Group = "444"; 
        Kind = ReviewerReview; 
        Authors = ["Ololoev"] }

    DocumentNameParser.parse "444-Ololoev-review.pdf" 
    |> should equal <| Some { 
        FileName = "444-Ololoev-review.pdf"; 
        Group = "444"; 
        Kind = AdvisorReview; 
        Authors = ["Ololoev"] }

[<Test>]
let ``Incorrect file names shall be correctly declined`` () =
    DocumentNameParser.parse "index.html" 
    |> should equal None

[<Test>]
let ``Works with multiple authors shall be parsed correctly`` () =
    DocumentNameParser.parse "242-Kizhnerov-Sokolvyak-Tuchina-slides.pdf" 
    |> should equal <| Some { 
        FileName = "242-Kizhnerov-Sokolvyak-Tuchina-slides.pdf"; 
        Group = "242"; 
        Kind = Slides; 
        Authors = ["Kizhnerov"; "Sokolvyak"; "Tuchina"] }