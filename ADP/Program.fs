namespace ADP

/// Entry point of a program.
module Main = 
    open System

    [<EntryPoint>]
    let main argv =
        let knowledgeBase = KnowledgeBase ()

        knowledgeBase 
        |> FilesProcessor.fill Environment.CurrentDirectory
        |> PdfInfoExtractor.extract
        |> JsonProcessor.read
        |> HtmlGenerator.generate
        |> JsonProcessor.generate
        |> ignore

        0
