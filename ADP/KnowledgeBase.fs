﻿namespace ADP

open System.Collections.Generic

/// Repository of all information known about qualification works.
type KnowledgeBase() = 
    let works = Dictionary<string, Diploma>()
    
    /// Adds given document to a repository creating new Diploma record if needed, or adds a new document 
    /// to an existing one.
    member v.Add (document : Document) =
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
    member v.AllWorks = works.Values :> Diploma seq

    member v.Merge (diploma: Diploma) =
        let id = diploma.ShortName
        if works.ContainsKey id then
            let existingDiploma = works.[id]
            if diploma.HasTitle then 
                existingDiploma.Title <- diploma.Title
            if diploma.HasAuthorName then
                existingDiploma.AuthorName <- diploma.AuthorName
            existingDiploma.ManuallyEdited <- true
        else
            diploma.ManuallyEdited <- true
            works.Add(id, diploma)
