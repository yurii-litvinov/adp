namespace ADP

/// Kind of a document related to a qualification work.
/// For second-course works only text and slides are applicable,
/// for third course advisor review is added, and for bachelor qualification work
/// all document kinds are applicable.
type DocumentKind =
    | Text
    | Slides
    | AdvisorReview
    | ReviewerReview

/// Document related to qualification work, stored as a file with special name format:
/// <group>-<transliterated surname of a student>-<document kind>.<extension>
/// For example, "444-Ololoev-report.pdf"
type Document = { 
    FileName : string; 
    Group : string;
    ShortName : string; 
    Kind : DocumentKind }

/// <summary>
/// All collected information about one qualification work. Includes list of related documents
/// and some metainformation to be used by generator.
/// </summary>
/// <param name="shortName">Transliterated student surname, used as an Id of a work throughout the system </param>
type Diploma(shortName: string) =
    member val Title = " " with get, set
    member val AuthorName = " " with get, set
    member v.ShortName = shortName
    member val Group = "" with get, set
    member val Course = 0 with get, set
    member val SourcesUrl = "" with get, set

    member val Text: Document option = None with get, set
    member val Slides: Document option = None with get, set
    member val AdvisorReview: Document option = None with get, set
    member val ReviewerReview: Document option = None with get, set

    /// Adds a new document to the diploma entry, updates metainformation if needed.
    member v.Add (document: Document) =
        if v.Group = "" then
            v.Group <- document.Group
            v.Course <- int (v.Group.Substring(0, 1))

        match document.Kind with
        | Text -> v.Text <- Some document
        | Slides -> v.Slides <- Some document
        | AdvisorReview -> v.AdvisorReview <- Some document
        | ReviewerReview -> v.ReviewerReview <- Some document

    /// Convenience method that reports if title of this work is known. To be used from .cshtml.
    member v.HasTitle = v.Title <> ""

    /// Convenience method that reports if author of this work is known. To be used from .cshtml.
    member v.HasName = v.AuthorName <> ""

    /// Convenience method that reports if URL of source files for this work is known. To be used from .cshtml.
    member v.HasSources = v.SourcesUrl <> ""

    /// Convenience method that reports if text for this work exists. To be used from .cshtml.
    member v.HasText = v.Text.IsSome

    /// Convenience method that reports if slides for this work exists. To be used from .cshtml.
    member v.HasSlides = v.Slides.IsSome

    /// Convenience method that reports if advisor review for this work exists. To be used from .cshtml.
    member v.HasAdvisorReview = v.AdvisorReview.IsSome

    /// Convenience method that reports if reviewer review for this work exists. To be used from .cshtml.
    member v.HasReviewerReview = v.ReviewerReview.IsSome
