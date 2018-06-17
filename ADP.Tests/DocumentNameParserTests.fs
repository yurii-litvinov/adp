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
        ShortName = "Ololoev" }

[<Test>]
let ``All document kinds shall be parsed`` () =
    DocumentNameParser.parse "444-Ololoev-slides.pdf" 
    |> should equal <| Some { 
        FileName = "444-Ololoev-slides.pdf"; 
        Group = "444"; 
        Kind = Slides; 
        ShortName = "Ololoev" }

    DocumentNameParser.parse "444-Ololoev-advisor-review.pdf" 
    |> should equal <| Some { 
        FileName = "444-Ololoev-advisor-review.pdf"; 
        Group = "444"; 
        Kind = AdvisorReview; 
        ShortName = "Ololoev" }

    DocumentNameParser.parse "444-Ololoev-reviewer-review.pdf" 
    |> should equal <| Some { 
        FileName = "444-Ololoev-reviewer-review.pdf"; 
        Group = "444"; 
        Kind = ReviewerReview; 
        ShortName = "Ololoev" }

[<Test>]
let ``Incorrect file names shall be correctly declined`` () =
    DocumentNameParser.parse "index.html" 
    |> should equal None
