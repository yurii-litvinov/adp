﻿module FilesProcessorTests

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
    let diploma = knowledgeBase.AllWorks |> Seq.find (fun (d: Diploma) -> d.ShortName = "Leonova")
    diploma.Text |> should equal None
    diploma.Slides.Value.Kind |> should equal Slides
    diploma.AdvisorReview |> should equal None
    diploma.ReviewerReview.Value.Kind |> should equal ReviewerReview
