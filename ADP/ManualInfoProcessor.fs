namespace ADP

/// Generates JSON file with all diploma records that have empty fields, to be filled by hand. 
/// Parses JSON file if it exists, filling gaps in a database with values from it (assuming they were correctly 
/// filled manually).
/// Also, this JSON takes precedence over automatic detection, so it can be used to override information 
/// about qualification works.
module ManualInfoProcessor =
    open Chiron
    open Chiron.Inference
    open Chiron.Operators
    open System.IO

    /// Helper type that allows to serialize information about diploma using Chiron JSON library. Looks ugly because
    /// Chiron 7 alpha was used at the moment of writing this, no more adequate API was available (or known).
    type DiplomaJson = 
        { 
          ShortName: string
          Title: string
          AuthorName: string 
          AdvisorName: string
          SourcesUrl: string 
          CommitterName: string
        }

        /// Serializes record into JSON object.
        static member ToJson (x: DiplomaJson) =
            Json.Encode.buildWith (fun x jObj ->
                                   jObj
                                   |> Json.Encode.required "shortName" x.ShortName
                                   |> Json.Encode.required "title" x.Title
                                   |> Json.Encode.required "authorName" x.AuthorName
                                   |> Json.Encode.required "advisorName" x.AdvisorName
                                   |> Json.Encode.required "sourcesUrl" x.SourcesUrl
                                   |> Json.Encode.required "committerName" x.CommitterName
                                  ) x

        /// Deserializes record from JSON object.
        static member FromJson (_: DiplomaJson) =
                let inner =
                    (fun shortName title authorName advisorName sourcesUrl committerName -> 
                        { 
                          ShortName = shortName
                          Title = title
                          AuthorName= authorName 
                          AdvisorName = advisorName 
                          SourcesUrl = sourcesUrl
                          CommitterName = committerName
                        })
                    <!> Json.Decode.required "shortName"
                    <*> Json.Decode.required "title"
                    <*> Json.Decode.required "authorName"
                    <*> Json.Decode.required "advisorName"
                    <*> Json.Decode.required "sourcesUrl"
                    <*> Json.Decode.required "committerName"
                Json.Decode.jsonObject >=> inner

        /// Creates new record from existing Diploma.
        static member FromDiploma (diploma: Diploma) =
            { 
                ShortName = diploma.ShortName
                Title = diploma.Title
                AuthorName = diploma.AuthorName
                AdvisorName = diploma.AdvisorName
                SourcesUrl = diploma.SourcesUrl
                CommitterName = diploma.CommitterName
            }

        /// Creates new Diploma from given record.
        static member ToDiploma (d: DiplomaJson) =
            let newDiploma = Diploma(d.ShortName)
            newDiploma.Title <- d.Title
            newDiploma.AuthorName <- d.AuthorName
            newDiploma.AdvisorName <- d.AdvisorName
            newDiploma.SourcesUrl <- d.SourcesUrl
            newDiploma.CommitterName <- d.CommitterName
            newDiploma

    /// Tells if info about given diploma needs to be generated in JSON file that allows to manually fill missing info.
    let private needRegenerate (diploma: Diploma) =
        not (
            diploma.HasTitle
            && diploma.HasAuthorName
            && diploma.HasAdvisorName
            && diploma.HasSourcesUrl
            && diploma.HasCommitterName
            )
        || diploma.ManuallyEdited

    /// Generates JSON file with diploma entries that can be filled by hand to provide additional information about 
    /// works, such as source code link and account name.
    let generate (knowledgeBase: KnowledgeBase) = 
        let toBeSerialized = knowledgeBase.AllWorks
                             |> Seq.filter needRegenerate
                             |> Seq.map DiplomaJson.FromDiploma
                             |> Seq.map Json.encode
                             |> Seq.toList

        let result = Json.Array toBeSerialized
        Json.formatWith JsonFormattingOptions.Pretty result
        |> fun s -> File.WriteAllText("works.json", s)

        knowledgeBase

    /// Reads existing JSON file with diploma entries if present.
    let read (knowledgeBase: KnowledgeBase) = 
        if File.Exists "works.json" then
            let parsed = File.ReadAllText "works.json"
                         |> Json.parse
            match parsed with
            | JFail reason -> failwith <| "Invalid JSON file" + reason.ToString ()
            | JPass (Array works) ->
                works 
                |> Seq.map Json.decode
                |> Seq.choose (function JPass o -> Some o | _ -> None)
                |> Seq.map DiplomaJson.ToDiploma
                |> Seq.iter knowledgeBase.Merge
            | _ -> failwith "Incorrect JSON file format"

        knowledgeBase
