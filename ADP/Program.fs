namespace ADP

/// Entry point of a program.
module Main = 
    open System

    [<EntryPoint>]
    let main argv =
        let knowledgeBase = KnowledgeBase ()

        knowledgeBase 
        |> FilesProcessor.fill Environment.CurrentDirectory
        |> HtmlGenerator.generate
        |> ignore

        0
