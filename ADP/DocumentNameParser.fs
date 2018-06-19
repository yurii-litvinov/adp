namespace ADP

open System.Text.RegularExpressions

/// Utility that helps to process file name and classify a file according to a fixed name format traditionally used
/// for qualification works of SE chair.
module DocumentNameParser =
    /// Regexp for file naming scheme in this format:
    /// <group>-<transliterated surname of a student>-<document kind>.<extension>
    /// For example, "444-Ololoev-report.pdf"
    let pattern = @"(?<Group>\d{3})-((?<ShortName>[a-z]+)-)+(?<Kind>(slides)|(report)|(advisor-review)|(reviewer-review))"
    let documentNameRegex = Regex(pattern, RegexOptions.IgnoreCase)

    /// Classifies files to document kinds.
    let private toDocumentKind = function
        | "report" -> Text
        | "slides" -> Slides
        | "advisor-review" -> AdvisorReview
        | "reviewer-review" -> ReviewerReview
        | _ -> failwith "Incorrect document kind, regex seems to be invalid"

    /// Parses given file name and produces corresponging Document entry if parsing was successful.
    /// Returns None if not.
    let parse fileName =
        let regexMatch = documentNameRegex.Match(fileName)
        if not regexMatch.Success then
            None
        else
            let group = regexMatch.Groups.["Group"].Value
            let authors = regexMatch.Groups.["ShortName"].Captures |> Seq.map (fun c -> c.Value) |> Seq.toList
            let kind = regexMatch.Groups.["Kind"].Value
            let fileName = (System.IO.FileInfo fileName).Name
            Some {FileName = fileName; Group = group; Authors = authors; Kind = toDocumentKind kind}

