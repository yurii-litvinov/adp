namespace ADP

open System.Collections.Generic

/// Repository of all information known about qualification works.
type KnowledgeBase() = 
    let works = Dictionary<string, Diploma>()
    
    /// Adds given document to a repository creating new Diploma record if needed, or adds a new document 
    /// to an existing one.
    member v.Add (document : Document) =
        let handle = document.ShortName
        let diploma = if works.ContainsKey handle then
                          works.[handle]
                      else
                          Diploma(handle)
        diploma.Add document
        works.[handle] <- diploma
        ()

    /// Returns all existing Diploma records.
    member v.AllWorks = works.Values :> Diploma seq