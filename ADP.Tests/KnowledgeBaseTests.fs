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
        Authors = ["Ololoev"] }

[<Test>]
let ``It shall be possible to add multiple documents to one tracked work`` () =
    let knowledgeBase = KnowledgeBase ()

    knowledgeBase.Add { 
        FileName = "444-Ololoev-report.pdf"; 
        Group = "444"; 
        Kind = Text; 
        Authors = ["Ololoev"] }

    knowledgeBase.Add { 
        FileName = "444-Ololoev-review.pdf"; 
        Group = "444"; 
        Kind = Text; 
        Authors = ["Ololoev"] }

[<Test>]
let ``It shall be possible to add multiple documents related to different works`` () =
    let knowledgeBase = KnowledgeBase ()

    knowledgeBase.Add { 
        FileName = "444-Ololoev-report.pdf"; 
        Group = "444"; 
        Kind = Text; 
        Authors = ["Ololoev"] }

    knowledgeBase.Add { 
        FileName = "444-Alibabaev-review.pdf"; 
        Group = "444"; 
        Kind = Text; 
        Authors = ["Alibabaev"] }

[<Test>]
let ``Manually entered information shall be merged correctly into knowledge base`` () =
    let knowledgeBase = KnowledgeBase ()
    knowledgeBase.Add { 
        FileName = "244-Zainullin-report.pdf"; 
        Group = "244"; 
        Kind = Text; 
        Authors = ["Zainullin"] }

    let record = knowledgeBase.AllWorks |> Seq.head
    record.Title <- "Редактор REAL.NET"

    let newInfo = new Diploma("Zainullin")
    newInfo.AuthorName <- "Зайнуллин Егор Евгеньевич"
    newInfo.AdvisorName <- "к.т.н., доц. Ю.В. Литвинов"
    newInfo.SourcesUrl <- "https://github.com/yurii-litvinov/REAL.NET"
    newInfo.CommitterName <- "egorzainullin"

    knowledgeBase.Merge newInfo

    let result = (ManualInfoProcessor.read knowledgeBase).AllWorks |> Seq.head
    result.Title |> should equal "Редактор REAL.NET"
    result.AuthorName |> should equal "Зайнуллин Егор Евгеньевич"
    result.AdvisorName |> should equal "к.т.н., доц. Ю.В. Литвинов"
    result.SourcesUrl |> should equal "https://github.com/yurii-litvinov/REAL.NET"
    result.CommitterName |> should equal "egorzainullin"
