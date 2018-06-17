namespace ADP

/// Module that walks given directory and fills knowledge base with documents found there.
module FilesProcessor = 
    open System.IO

    /// Walks given directory and fills knowledge base with documents found there.
    let fill dir (knowledgeBase: KnowledgeBase) =
        let files = Directory.GetFiles dir
        files 
        |> Seq.choose DocumentNameParser.parse
        |> Seq.iter knowledgeBase.Add

        knowledgeBase
