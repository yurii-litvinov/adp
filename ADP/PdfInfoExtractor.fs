namespace ADP

/// Adds information that can be extracted from .pdf into a knowledge base. Uses some heuristics based on a fact
/// that title page is somewhat standartised.
module PdfInfoExtractor =
    open System.IO
    open System.Text.RegularExpressions

    /// Possible names of chairs and education programs that have to appear on the top of a title page.
    let private programs = [
        "Кафедра системного программирования";
        "Кафедра cистемного программирования";  // That's not the same string. Unicode is kind of insane.
        ".?Системное программирование.?";
        "Информационные системы и базы данных";
        "Кафедра информатики";
        "Программная инженерия";
        "Направление программной инженерии";

        // Some exceptions for 2nd course, which has not profile and chair yet
        "группа";
        "систем";
        "университет";
        "университет»"
    ]

    /// Symbols that are allowed to be in the title.
    let private allowedTitleSymbols = [@"\w"; @"\s"; @"\n"; "\p{P}"]

    /// List of known qualification work kinds, it shall appear in the middle of a title page.
    let private workKinds = [
        "Выпускная";
        "Бакалаврская"
        "Дипломная";
        "Курсовая";
    ]

    /// Creates a list regexp patterns that will be matched against extracted text.
    let private buildPatterns () =
        let alt list = list |> List.reduce (fun s1 s2 -> s1 + "|" + s2)
        let titlePattern = alt allowedTitleSymbols
        let titlePattern = "(?<Title>(" + titlePattern + ")+)"
        let worksPattern = "(" + (alt workKinds) + ")"
        let regularPatterns = 
            programs 
            |> List.map (fun s -> s + @"\s*\n+(?<Name>\w+\s\w+\s\w+)\s*\n+" + titlePattern + @"\n+" + worksPattern)
        let workKindOmittedPatterns =
            programs 
            |> List.map (fun s -> s + @"\s*\n+(?<Name>\w+\s\w+\s\w+)\s*\n+" + titlePattern + @"\n+" + "Научный руководитель")
        regularPatterns @ workKindOmittedPatterns
 
    /// Tries to open given file as a PDF document and extract all possible information from it and put it into given
    /// Diploma record.
    let private extractFromPdf (diploma: Diploma) (file: string) =
        let text = PdfTextExtractor.getFirstPageText file
        let regexps = buildPatterns () |> List.map (fun pattern -> Regex(pattern, RegexOptions.IgnoreCase))
        let mutable ``done`` = false
        for regexp in regexps do
            if ``done`` then ()
            else
                let regexMatch = regexp.Match text
                if not regexMatch.Success then
                    ()
                else
                    let name = regexMatch.Groups.["Name"].Value
                    let title = regexMatch.Groups.["Title"].Value
                    let title = title.Replace('\n', ' ')
                    diploma.Title <- title
                    diploma.AuthorName <- name
                    ``done`` <- true

        let advisorPattern = @"Научный руководитель.*\n(?<Advisor>.*)\n"
        let advisorRegex = Regex(advisorPattern, RegexOptions.IgnoreCase)
        let advisorMatch = advisorRegex.Match text
        if not advisorMatch.Success then
            ()
        else
            let name = advisorMatch.Groups.["Advisor"].Value
            diploma.AdvisorName <- name

        let consultantPattern = @"Консультант.*\n(?<Consultant>.*)\n"
        let consultantRegex = Regex(consultantPattern, RegexOptions.IgnoreCase)
        let consultantMatch = consultantRegex.Match text
        if not consultantMatch.Success then
            ()
        else
            let name = consultantMatch.Groups.["Consultant"].Value
            diploma.ConsultantName <- name

    /// Checks that given diploma has PDF document with report and then tries to extract information from it.
    let private extractForDiploma (diploma: Diploma) =
        async {
            if not diploma.HasText then
                ()
            else
                let file = diploma.Text.Value.FileName
                if FileInfo(file).Extension <> ".pdf" then 
                    ()
                else
                    extractFromPdf diploma file 
        }

    /// Iterates all known works in a base, trying to look inside a report .pdf file and extract title and author name.
    /// Results are stored again in a knowledge base.
    let extract (knowledgeBase: KnowledgeBase) =
        // All parallel chunks of this work actually modify the same knowledge base, but no race conditions are 
        // possible since every thread modifies its own diploma and they are guaranteed to be unique.
        knowledgeBase.AllWorks |> Seq.map extractForDiploma |> Async.Parallel |> Async.RunSynchronously |> ignore
        knowledgeBase
