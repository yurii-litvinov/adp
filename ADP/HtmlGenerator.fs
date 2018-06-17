namespace ADP

/// Generates of HTML metainformation table based on a knowledge collected about qualification works.
module HtmlGenerator =
    open System.IO
    open System
    open RazorLight

    /// Generates html page with qualification works according to a template
    let generate (knowledgeBase: KnowledgeBase) =
        let engine = RazorLightEngineBuilder()
                        .UseFilesystemProject(AppDomain.CurrentDomain.BaseDirectory)
                        .UseMemoryCachingProvider()
                        .Build()

        let result = engine.CompileRenderAsync("DiplomaTemplate.cshtml", knowledgeBase).Result
        let result = result.Trim()
        File.WriteAllText("Diploma.html", result)
        knowledgeBase
