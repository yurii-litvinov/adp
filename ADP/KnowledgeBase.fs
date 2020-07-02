namespace ADP

open System.Collections.Generic

/// Repository of all information known about qualification works.
type KnowledgeBase() = 
    let works = Dictionary<string, Diploma>()

    /// A list of files that are failed to match any of known types of documents, used for error reporting.
    let mutable unknownFiles = []

    /// Represents a path to a qualification work on a server, used in .cshtml templates.
    member val FolderOnServer = "" with get, set

    /// Represents state of diploma processor. True if it is a first pass and knowledge base was just generated,
    /// false if this is a second pass after manual editing and we are ready to generate HTML.
    member val FirstPass = true with get, set

    /// Academic course of all works in this base. Determines used template, must be set manually in generated config.
    member val Course = 2 with get, set
    
    /// Adds given document to a repository creating new Diploma record if needed, or adds a new document 
    /// to an existing one.
    member this.Add (document : Document) =
        let authors = document.Authors
        for author in authors do
            let diploma = if works.ContainsKey author then
                              works.[author]
                          else
                              Diploma(author)
            diploma.Add document
            works.[author] <- diploma
        ()

    /// Adds a file not recognized by document name parser for error reporting.
    member this.AddUnknownFile (fileName: string) =
       if not (fileName.EndsWith ".json") && not (fileName.EndsWith "out.html") then
           unknownFiles <- fileName :: unknownFiles

    /// Returns a collection of files not recognized by document name parser, for error reporting.
    member this.UnknownFiles =
        unknownFiles |> List.toSeq

    /// Returns all existing Diploma records.
    member this.AllWorks = (works.Values :> Diploma seq) |> Seq.sortBy (fun d -> d.AuthorName)

    /// Merges data from a given record into a database, marking record as manually edited. If there was no such
    /// record, given record is simply added to a database.
    member this.Merge (diploma: Diploma) =
        let id = diploma.ShortName
        if works.ContainsKey id then
            let existingDiploma = works.[id]
            existingDiploma.Merge diploma
        else
            diploma.ManuallyEdited <- true
            works.Add(id, diploma)
