﻿namespace ADP

/// Allows to manually specify information about scientific advisors to be used consistently in all works.
/// On a first run this module generates JSON file with empty advisor info template, then it supposed to be filled
/// by hand and on a second run all works with matched advisors will use info provided in advisors JSON instead of
/// autodetected ones.
module AdvisorsInfoProcessor =

    open Chiron
    open Chiron.Inference
    open Chiron.Operators
    open System.IO
    open System.Text.RegularExpressions
    open System

    let private file = "advisors.json"

    type AdvisorJson = 
        { 
          Pattern: string
          Name: string
        }

        static member ToJson (x: AdvisorJson) =
            Json.Encode.buildWith (fun x jObj ->
                                   jObj
                                   |> Json.Encode.required "pattern" x.Pattern
                                   |> Json.Encode.required "name" x.Name
                                  ) x
                              
        static member FromJson (_: AdvisorJson) =
                let inner =
                    (fun pattern name -> { Pattern = pattern; Name = name })
                    <!> Json.Decode.required "pattern"
                    <*> Json.Decode.required "name"
                Json.Decode.jsonObject >=> inner

        static member FromName name =
            { Pattern = name; Name = name }

        static member Apply (knowledgeBase: KnowledgeBase) (record: AdvisorJson) =
            let regex = Regex(record.Pattern, RegexOptions.IgnoreCase)
            let applyToDiploma (d: Diploma) =
                if regex.IsMatch(d.AdvisorName) then
                    d.AdvisorName <- record.Name
            knowledgeBase.AllWorks 
            |> Seq.iter applyToDiploma

    let generate (knowledgeBase: KnowledgeBase) = 
        if not <| File.Exists file then
            File.Copy(
                Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "advisors.json"), 
                Path.Combine(Environment.CurrentDirectory, "advisors.json")
            )

        knowledgeBase

    let read (knowledgeBase: KnowledgeBase) = 
        if File.Exists file then
            let parsed = File.ReadAllText file
                         |> Json.parse
            match parsed with
            | JFail reason -> failwith <| "Invalid advisor info JSON file" + reason.ToString ()
            | JPass (Array advisors) ->
                advisors 
                |> Seq.map Json.decode
                |> Seq.choose (function JPass o -> Some o | _ -> None)
                |> Seq.iter (AdvisorJson.Apply knowledgeBase)
            | _ -> failwith "Incorrect JSON file format"

        knowledgeBase


