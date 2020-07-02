namespace ADP

/// Parses JSON with global configuration parameter. If JSON is not present, generates new one with default parameters
/// to be filled manually.
module ConfigProcessor =

    open Chiron
    open Chiron.Inference
    open Chiron.Operators
    open System.IO

    /// File where global configuration shall be stored.
    let private file = "config.json"

    /// Helper record for serialization-deserialization into JSON using Chiron library.
    type ConfigJson = 
        { FolderOnServer: string;
          Course: int }

        /// Serializes record into JSON object.
        static member ToJson (x: ConfigJson) =
            Json.Encode.buildWith (fun x jObj ->
                                   jObj
                                   |> Json.Encode.required "folderOnServer" x.FolderOnServer
                                   |> Json.Encode.required "course" x.Course
                                  ) x

        /// Deserializes record from JSON object.
        static member FromJson (_: ConfigJson) =
                let inner =
                    (fun folder course -> 
                        { 
                            FolderOnServer = folder;
                            Course = course
                            }
                    )
                    <!> Json.Decode.required "folderOnServer"
                    <*> Json.Decode.required "course"
                Json.Decode.jsonObject >=> inner

    /// Generates new config  file with default settings to be edited manually, if no file exists already.
    let generate (knowledgeBase: KnowledgeBase) = 
        if not <| File.Exists file then
            let defaultFolder = "YearlyProjects/spring-2018/244/"
            Json.encode { 
                  FolderOnServer = defaultFolder 
                  Course = 2
                }
            |> Json.formatWith JsonFormattingOptions.Pretty
            |> fun s -> File.WriteAllText(file, s)

            knowledgeBase.FirstPass <- true
        else
            knowledgeBase.FirstPass <- false

        knowledgeBase

    /// Loads existing config file, if present.
    let read (knowledgeBase: KnowledgeBase) = 
        if File.Exists file then
            let parsed = File.ReadAllText file
                         |> Json.parse
            match parsed with
            | JFail reason -> failwith <| "Invalid config JSON file" + reason.ToString ()
            | JPass config ->
                match Json.decode config with
                | JPass config -> 
                    knowledgeBase.FolderOnServer <- config.FolderOnServer
                    knowledgeBase.Course <- config.Course
                | _ -> failwith "Invalid config JSON file contents"

        knowledgeBase
