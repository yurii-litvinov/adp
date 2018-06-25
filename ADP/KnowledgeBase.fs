namespace ADP

open System.Collections.Generic

/// Repository of all information known about qualification works.
type KnowledgeBase() = 
    let works = Dictionary<string, Diploma>()

    member val FolderOnServer = "" with get, set
    
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

    /// Returns all existing Diploma records.
    member this.AllWorks = (works.Values :> Diploma seq) |> Seq.sortBy (fun d -> d.AuthorName)

    /// Returns academic course of all the works in a base. If there are different courses, throws an exception.
    member this.Course = 
        works.Values
            |> Seq.fold
                (fun course (work: Diploma) -> 
                    if course = 0 then
                        work.Course
                    elif course <> work.Course then
                        failwith "There are works from different courses in a folder"
                    else
                        course)
                0

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
