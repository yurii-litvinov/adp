namespace ADP

/// Prints collected diagnostics to a console.
module DiagnosticsPrinter =

    /// Prints collected diagnostics from knowledge base to a console.
    let print (knowledgeBase: KnowledgeBase) = 
        if not (knowledgeBase.UnknownFiles |> Seq.isEmpty) then 
            printfn "Warning! Those files did not match any known pattern:"
            knowledgeBase.UnknownFiles |> Seq.iter (printfn "%s")
        knowledgeBase
