namespace ADP

/// Parses JSON with global configuration parameter. If JSON is not present, generates new one with default parameters
/// to be filled manually.
module ConfigProcessor =

    open Chiron
    open Chiron.Inference
    open Chiron.Operators
    open System.IO

    let private file = "config.json"

    type ConfigJson = 
        { FolderOnServer: string }

        static member ToJson (x: ConfigJson) =
            Json.Encode.buildWith (fun x jObj ->
                                   jObj
                                   |> Json.Encode.required "folderOnServer" x.FolderOnServer
                                  ) x
                              
        static member FromJson (_: ConfigJson) =
                let inner =
                    (fun folder -> { FolderOnServer = folder })
                    <!> Json.Decode.required "folderOnServer"
                Json.Decode.jsonObject >=> inner

    let generate (knowledgeBase: KnowledgeBase) = 
        if not <| File.Exists file then
            Json.encode { FolderOnServer = "YearlyProjects/spring-2018/244/" }
            |> Json.formatWith JsonFormattingOptions.Pretty
            |> fun s -> File.WriteAllText(file, s)

        knowledgeBase

    let read (knowledgeBase: KnowledgeBase) = 
        if File.Exists file then
            let parsed = File.ReadAllText file
                         |> Json.parse
            match parsed with
            | JFail _ -> failwith "Invalid config JSON file"
            | JPass config ->
                match Json.decode config with
                | JPass config -> knowledgeBase.FolderOnServer <- config.FolderOnServer
                | _ -> failwith "Invalid config JSON file contents"

        knowledgeBase




