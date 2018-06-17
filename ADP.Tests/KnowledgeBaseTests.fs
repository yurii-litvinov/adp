module KnowledgeBaseTests

open NUnit.Framework
open FsUnit
open ADP

[<Test>]
let ``It shall be possible to add a document`` () =
    let knowledgeBase = KnowledgeBase ()
    knowledgeBase.Add { 
        FileName = "444-Ololoev-report.pdf"; 
        Group = "444"; 
        Kind = Text; 
        ShortName = "Ololoev" }

[<Test>]
let ``It shall be possible to add multiple documents to one tracked work`` () =
    let knowledgeBase = KnowledgeBase ()

    knowledgeBase.Add { 
        FileName = "444-Ololoev-report.pdf"; 
        Group = "444"; 
        Kind = Text; 
        ShortName = "Ololoev" }

    knowledgeBase.Add { 
        FileName = "444-Ololoev-review.pdf"; 
        Group = "444"; 
        Kind = Text; 
        ShortName = "Ololoev" }

[<Test>]
let ``It shall be possible to add multiple documents related to different works`` () =
    let knowledgeBase = KnowledgeBase ()

    knowledgeBase.Add { 
        FileName = "444-Ololoev-report.pdf"; 
        Group = "444"; 
        Kind = Text; 
        ShortName = "Ololoev" }

    knowledgeBase.Add { 
        FileName = "444-Alibabaev-review.pdf"; 
        Group = "444"; 
        Kind = Text; 
        ShortName = "Alibabaev" }