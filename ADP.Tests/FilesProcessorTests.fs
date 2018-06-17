module FilesProcessorTests

open NUnit.Framework
open FsUnit
open ADP
open System
open System.IO

[<Test>]
let ``When ran on a test dir, shall provide expected results`` () =
    let knowledgeBase = KnowledgeBase ()
    let dir = Path.Combine(Environment.CurrentDirectory, "TestDir")
    let knowledgeBase = knowledgeBase |> FilesProcessor.fill dir
    knowledgeBase.WorksList |> should haveCount 1
    let diploma = Seq.head knowledgeBase.WorksList
    diploma.Group |> should equal "441"
    diploma.Course |> should equal 4
    diploma.Text |> should equal None
    diploma.Slides.Value.Kind |> should equal Slides
    diploma.AdvisorReview |> should equal None
    diploma.ReviewerReview.Value.Kind |> should equal ReviewerReview
