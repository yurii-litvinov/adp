namespace ADP

/// Module that walks given directory and fills knowledge base with documents found there.
module FilesProcessor = 
    open System.IO

    /// Walks given directory and fills knowledge base with documents found there.
    let fill dir (knowledgeBase: KnowledgeBase) =
        let files = Directory.GetFiles dir
        files 
        |> Seq.map DocumentNameParser.parse
        |> Seq.iter (function 
                     | Choice1Of2 document -> knowledgeBase.Add document
                     | Choice2Of2 fileName -> knowledgeBase.AddUnknownFile fileName
                    )

        knowledgeBase
