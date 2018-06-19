module PdfInfoExtractorTests

open NUnit.Framework
open FsUnit
open ADP

[<Test>]
let ``PDF Extractor shall correctly extract title and author name of a diploma`` () =
    let knowledgeBase = KnowledgeBase ()
    let tankovDiploma = { 
        FileName = "TestDir/444-Tankov-report.pdf"; 
        Group = "444"; 
        Kind = Text; 
        Authors = ["Tankov"] }

    knowledgeBase.Add tankovDiploma
    knowledgeBase |> PdfInfoExtractor.extract |> ignore

    let extractedDiploma = knowledgeBase.AllWorks |> Seq.head
    extractedDiploma.Title |> should equal "Основанный на данных синтез кода в IntelliJ IDEA"
    extractedDiploma.AuthorName |> should equal "Танков Владислав Дмитриевич"

[<Test>]
let ``Strange documents shall be parsed correctly`` () =
    let knowledgeBase = KnowledgeBase ()
    let diploma = { 
        FileName = "TestDir/441-Konovalova-report.pdf"; 
        Group = "441"; 
        Kind = Text; 
        Authors = ["Konovalova"] }
    
    knowledgeBase.Add diploma
    knowledgeBase |> PdfInfoExtractor.extract |> ignore

    let extractedDiploma = knowledgeBase.AllWorks |> Seq.head
    extractedDiploma.HasTitle |> should be True
