namespace ADP

/// Generates of HTML metainformation table based on a knowledge collected about qualification works.
module HtmlGenerator =
    open System.IO
    open System
    open RazorLight

    let private templates = 
        Map.ofList
            [2, "CourseWork2ndCourseTemplate.cshtml";
             3, "CourseWork3rdCourseTemplate.cshtml";
             4, "DiplomaTemplate.cshtml"]

    let private selectTemplate (knowledgeBase: KnowledgeBase) =
        if Seq.isEmpty knowledgeBase.AllWorks then
            (Seq.head templates).Value
        else
            let course = knowledgeBase.AllWorks 
                         |> Seq.fold
                             (fun course (work: Diploma) -> 
                                 if course = 0 then
                                     work.Course
                                 elif course <> work.Course then
                                     failwith "There are works from different courses in a folder"
                                 else
                                     course)
                             0

            if not <| templates.ContainsKey course then
                failwith "No template for these works defined"

            templates.[course]

    /// Generates html page with qualification works according to a template
    let generate (knowledgeBase: KnowledgeBase) =
        let engine = RazorLightEngineBuilder()
                        .UseFilesystemProject(AppDomain.CurrentDomain.BaseDirectory)
                        .UseMemoryCachingProvider()
                        .Build()

        let template = selectTemplate knowledgeBase

        let result = engine.CompileRenderAsync(template, knowledgeBase).Result
        let result = result.Trim()
        File.WriteAllText("out.html", result)
        knowledgeBase
