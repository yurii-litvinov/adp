namespace ADP

/// Adds information that can be extracted from .pdf into a knowledge base. Uses some heuristics based on a fact
/// that title page is somewhat standartised.
module PdfInfoExtractor =
    open System.IO
    open System.Text.RegularExpressions
    
    let private extractFromPdf (diploma: Diploma) (file: string) =
        let text = PdfTextExtractor.getFirstPageText file
        let pattern = @"Кафедра системного программирования\n(?<Name>\w+\s\w+\s\w+)\n(?<Title>(\w|\s|\n)+)\nВыпускная"
        let textRegex = Regex(pattern, RegexOptions.IgnoreCase)
        let regexMatch = textRegex.Match(text)
        if not regexMatch.Success then
            ()
        else
            let name = regexMatch.Groups.["Name"].Value
            let title = regexMatch.Groups.["Title"].Value
            let title = title.Replace('\n', ' ')
            diploma.Title <- title
            diploma.AuthorName <- name

    let private extractForDiploma (diploma: Diploma) =
        if not diploma.HasText then
            ()
        else
            let file = diploma.Text.Value.FileName
            if FileInfo(file).Extension <> ".pdf" then 
                ()
            else
                extractFromPdf diploma file 

    /// Iterates all known works in a base, trying to look inside a report .pdf file and extract title and author name.
    /// Results are stored again in a knowledge base.
    let extract (knowledgeBase: KnowledgeBase) =
        knowledgeBase.AllWorks |> Seq.iter extractForDiploma
        knowledgeBase
