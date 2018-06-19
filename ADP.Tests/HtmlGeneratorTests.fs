module HtmlGeneratorTests

open NUnit.Framework
open FsUnit
open ADP
open System.IO
open System

[<Test>]
let ``Empty knowledge base shall generate HTML stub`` () =
    let knowledgeBase = KnowledgeBase ()
    knowledgeBase |> HtmlGenerator.generate |> ignore
    FileAssert.Exists "out.html"

[<Test>]
let ``Test knowledge base shall generate something meaningful`` () =
    let knowledgeBase = KnowledgeBase ()
    let dir = Path.Combine(Environment.CurrentDirectory, "TestDir")
    knowledgeBase |> FilesProcessor.fill dir |> HtmlGenerator.generate |> ignore
    FileAssert.Exists "Diploma.html"
    let text = File.ReadAllText "out.html"
    text |> should contain "Leonova"