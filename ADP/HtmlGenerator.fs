namespace ADP

/// Generates of HTML metainformation table based on a knowledge collected about qualification works.
module HtmlGenerator =
    open System.IO
    open System
    open RazorLight

    /// Known Razor templates for different courses.
    let private templates = 
        Map.ofList
            [2, "CourseWork2ndCourseTemplate.cshtml";
             3, "CourseWork3rdCourseTemplate.cshtml";
             4, "DiplomaTemplate.cshtml"]

    /// Selects appropriate template to be used for this knowledge base. Assumes that all works in this base are
    /// of the same academic course.
    let private selectTemplate (knowledgeBase: KnowledgeBase) =
        let course = knowledgeBase.Course
        if not <| templates.ContainsKey course then
            failwith "No template for these works defined"

        templates.[course]

    /// Generates html page with qualification works according to a template
    let generate (knowledgeBase: KnowledgeBase) =
        if not knowledgeBase.FirstPass then
            let engine = RazorLightEngineBuilder()
                            .UseFilesystemProject(AppDomain.CurrentDomain.BaseDirectory)
                            .UseMemoryCachingProvider()
                            .Build()

            let template = selectTemplate knowledgeBase

            let result = engine.CompileRenderAsync(template, knowledgeBase).Result
            let result = result.Trim()
            File.WriteAllText("out.html", result)
        knowledgeBase
