namespace ADP

/// Entry point of a program.
module Main = 
    open System

    [<EntryPoint>]
    let main argv =
        let knowledgeBase = KnowledgeBase ()

        knowledgeBase
        |> ConfigProcessor.read
        |> FilesProcessor.fill Environment.CurrentDirectory
        |> ConfigProcessor.generate
        |> PdfInfoExtractor.extract
        |> ManualInfoProcessor.read
        |> AdvisorsInfoProcessor.read

        |> HtmlGenerator.generate
        |> ManualInfoProcessor.generate
        |> AdvisorsInfoProcessor.generate
        |> ignore

        0
