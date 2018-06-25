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
        { FolderOnServer: string }

        /// Serializes record into JSON object.
        static member ToJson (x: ConfigJson) =
            Json.Encode.buildWith (fun x jObj ->
                                   jObj
                                   |> Json.Encode.required "folderOnServer" x.FolderOnServer
                                  ) x

        /// Deserializes record from JSON object.
        static member FromJson (_: ConfigJson) =
                let inner =
                    (fun folder -> { FolderOnServer = folder })
                    <!> Json.Decode.required "folderOnServer"
                Json.Decode.jsonObject >=> inner

    /// List of default server folders by academic course relative to most common position of generated index document.
    let private defaultFolders = Map.ofList [
                                     2, "YearlyProjects/spring-2018/244/";
                                     3, "344";
                                     4, "bmo"
                                 ]

    /// Generates new config  file with default settings to be edited manually, if no file exists already.
    let generate (knowledgeBase: KnowledgeBase) = 
        if not <| File.Exists file then
            let defaultFolder = 
                if knowledgeBase.AllWorks |> Seq.isEmpty then
                    defaultFolders |> Seq.head |> fun v -> v.Value
                else
                    defaultFolders.[knowledgeBase.Course]
            Json.encode { FolderOnServer = defaultFolder }
            |> Json.formatWith JsonFormattingOptions.Pretty
            |> fun s -> File.WriteAllText(file, s)

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
                | JPass config -> knowledgeBase.FolderOnServer <- config.FolderOnServer
                | _ -> failwith "Invalid config JSON file contents"

        knowledgeBase
