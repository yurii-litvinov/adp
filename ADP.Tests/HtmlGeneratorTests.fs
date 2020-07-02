module HtmlGeneratorTests

open NUnit.Framework
open FsUnit
open ADP
open System.IO
open System

let mutable knowledgeBase = KnowledgeBase ()

let get = function
| Choice1Of2 document -> document
| _ -> failwith "document name failed to parse"

[<SetUp>]
let setUp () =
    knowledgeBase <- KnowledgeBase ()
    knowledgeBase.FirstPass <- false

[<Test>]
let ``First-pass knowledge base shall generate nothing`` () =
    knowledgeBase.FirstPass <- true
    knowledgeBase |> HtmlGenerator.generate |> ignore
    FileAssert.Exists "out.html"


[<Test>]
let ``Empty knowledge base shall generate HTML stub`` () =
    knowledgeBase |> HtmlGenerator.generate |> ignore
    FileAssert.Exists "out.html"

[<Test>]
let ``Test knowledge base shall generate something meaningful`` () =
    knowledgeBase.Course <- 4
    let dir = Path.Combine(Environment.CurrentDirectory, "TestDir")
    knowledgeBase |> FilesProcessor.fill dir |> HtmlGenerator.generate |> ignore
    let text = File.ReadAllText "out.html"
    text |> should contain "Leonova"

[<Test>]
let ``2nd course template generates something meaningful`` () =
    knowledgeBase.Course <- 2
    let work = DocumentNameParser.parse "Zainullin-report.pdf"
    knowledgeBase.Add (get work)
    HtmlGenerator.generate knowledgeBase |> ignore
    let text = File.ReadAllText "out.html"
    text |> should contain "Zainullin"

[<Test>]
let ``3rd course template generates something meaningful`` () =
    knowledgeBase.Course <- 3
    let work = DocumentNameParser.parse "Batoev-report.pdf"
    knowledgeBase.Add (get work)
    HtmlGenerator.generate knowledgeBase |> ignore
    let text = File.ReadAllText "out.html"
    text |> should contain "Batoev"
