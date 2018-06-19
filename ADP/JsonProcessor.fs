﻿namespace ADP

/// Generates JSON file with all diploma records that have empty fields, to be filled by hand. 
/// Parses JSON file if it exists, filling gaps in a database with values from it (assuming they were correctly 
/// filled manually).
/// Also, this JSON takes precedence over automatic detection, so it can be used to override information 
/// about qualification works.
module JsonProcessor =
    open Chiron
    open Chiron.Inference
    open Chiron.Operators
    open System.IO

    type DiplomaJson = 
        { ShortName: string
          Title: string
          AuthorName: string }

        static member ToJson (x: DiplomaJson) =
            Json.Encode.buildWith (fun x jObj ->
                                   jObj
                                   |> Inference.Json.Encode.required "shortName" x.ShortName
                                   |> Inference.Json.Encode.required "title" x.Title
                                   |> Inference.Json.Encode.required "authorName" x.AuthorName
                                  ) x
                              
        static member FromJson (_: DiplomaJson) =
                let inner =
                    (fun s t a -> { ShortName = s; Title = t; AuthorName= a })
                    <!> Inference.Json.Decode.required "shortName"
                    <*> Json.Decode.required "title"
                    <*> Json.Decode.required "authorName"
                Json.Decode.jsonObject >=> inner

        static member FromDiploma (diploma: Diploma) =
            { 
                ShortName = diploma.ShortName;
                Title = diploma.Title;
                AuthorName = diploma.AuthorName
            }

        static member ToDiploma (d: DiplomaJson) =
            let newDiploma = Diploma(d.ShortName)
            newDiploma.Title <- d.Title
            newDiploma.AuthorName <- d.AuthorName
            newDiploma

    let generate (knowledgeBase: KnowledgeBase) = 
        let toBeSerialized = knowledgeBase.AllWorks
                             |> Seq.filter (fun d -> not d.HasTitle || d.ManuallyEdited)
                             |> Seq.map DiplomaJson.FromDiploma
                             |> Seq.map Json.encode
                             |> Seq.toList

        let result = Json.Array toBeSerialized
        Json.formatWith JsonFormattingOptions.Pretty result
        |> fun s -> File.WriteAllText("works.json", s)

        knowledgeBase

    let read (knowledgeBase: KnowledgeBase) = 
        if File.Exists "works.json" then
            let parsed = File.ReadAllText "works.json"
                         |> Json.parse
            match parsed with
            | JFail _ -> failwith "Invalid JSON file"
            | JPass (Array works) ->
                works 
                |> Seq.map Json.decode
                |> Seq.choose (function JPass o -> Some o | _ -> None)
                |> Seq.map DiplomaJson.ToDiploma
                |> Seq.iter knowledgeBase.Merge
            | _ -> failwith "Incorrect JSON file format"

        knowledgeBase
