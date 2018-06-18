module PdfInfoExtractorTests

open NUnit.Framework
open FsUnit
open ADP
open System.IO
open System

[<Test>]
let ``PDF Extractor shall correctly extract title and author name of a diploma`` () =
    let knowledgeBase = KnowledgeBase ()
    let tankovDiploma = { 
        FileName = "TestDir/444-Tankov-report.pdf"; 
        Group = "444"; 
        Kind = Text; 
        ShortName = "Tankov" }

    knowledgeBase.Add tankovDiploma
    knowledgeBase |> PdfInfoExtractor.extract |> ignore

    let extractedDiploma = knowledgeBase.AllWorks |> Seq.head
    extractedDiploma.Title |> should equal "Основанный на данных синтез кода в IntelliJ IDEA"
    extractedDiploma.AuthorName |> should equal "Танков Владислав Дмитриевич"
 