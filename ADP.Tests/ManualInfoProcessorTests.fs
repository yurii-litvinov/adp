module ManualInfoProcessorTests

open NUnit.Framework
open FsUnit
open ADP
open System.IO

[<Test>]
let ``Information about diploma shall be extracted correctly from a test JSON document`` () =
    let knowledgeBase = KnowledgeBase ()
    let result = (ManualInfoProcessor.read knowledgeBase).AllWorks |> Seq.head
    result.ShortName |> should equal "Zainullin"
    result.Title |> should equal "Редактор REAL.NET"
    result.AuthorName |> should equal "Зайнуллин Егор Евгеньевич"
    result.AdvisorName |> should equal "к.т.н., доц. Ю.В. Литвинов"
    result.SourcesUrl |> should equal "https://github.com/yurii-litvinov/REAL.NET"
    result.CommitterName |> should equal "egorzainullin"

[<Test>]
let ``Information about diploma shall be regenerated as it is marked as manually edited`` () =
    let knowledgeBase = KnowledgeBase ()
    knowledgeBase 
        |> ManualInfoProcessor.read
        |> ManualInfoProcessor.generate
        |> ignore

    let result = File.ReadAllText "works.json"
    result |> should contain "Zainullin"
    result |> should contain "Редактор REAL.NET"
    result |> should contain "Зайнуллин Егор Евгеньевич"
    result |> should contain "к.т.н., доц. Ю.В. Литвинов"
    result |> should contain "https://github.com/yurii-litvinov/REAL.NET"
    result |> should contain "egorzainullin"
