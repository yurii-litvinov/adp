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
    Authors : string list; 
    Kind : DocumentKind }

/// <summary>
/// All collected information about one qualification work. Includes list of related documents
/// and some metainformation to be used by generator.
/// </summary>
/// <param name="shortName">Transliterated student surname, used as an Id of a work throughout the system </param>
type Diploma(shortName: string) =
    
    /// Transliterated student surname, used as an Id of a work throughout the system.
    member v.ShortName = shortName
    
    /// Title of the diploma.
    member val Title = "" with get, set

    /// Full name of the author (<Surname> <First name> <Middle name>).
    member val AuthorName = "" with get, set

    /// Name of a scientific advisor (with degree and position).
    member val AdvisorName = "" with get, set

    /// Name of a technical consultant.
    member val ConsultantName = "" with get, set

    /// Number of an academic group (string because sometimes academic groups have complex numeration like "14Б07-ММ").
    member val Group = "" with get, set

    /// Number of academic course (1-4 for bachelors, 5-6 for masters).
    member val Course = 0 with get, set

    /// Link to source code of a diploma.
    member val SourcesUrl = "" with get, set

    /// Account name of the author, used to identify his/her contribution in case of multi-author project.
    member val CommitterName = "" with get, set

    /// Flag that is set internally when some information for this diploma was specified manually instead of 
    /// being extracted from existing .pdf files.
    member val ManuallyEdited = false with get, set

    /// Text of a diploma, if present.
    member val Text: Document option = None with get, set

    /// Slides from the defence of this diploma, if present.
    member val Slides: Document option = None with get, set

    /// Scientific advisor review, if present.
    member val AdvisorReview: Document option = None with get, set

    /// Reviewer review, if present.
    member val ReviewerReview: Document option = None with get, set

    /// Convenience method that reports if title of this work is known. To be used from .cshtml.
    member v.HasTitle = v.Title <> ""

    /// Convenience method that reports if author of this work is known. To be used from .cshtml.
    member v.HasAuthorName = v.AuthorName <> ""

    /// Convenience method that reports if scientific advisor of this work is known. To be used from .cshtml.
    member v.HasAdvisorName = v.AdvisorName <> ""

        /// Convenience method that reports if scientific advisor of this work is known. To be used from .cshtml.
    member v.HasConsultantName = v.ConsultantName <> ""

    /// Convenience method that reports if URL of source files for this work is known. To be used from .cshtml.
    member v.HasSourcesUrl = v.SourcesUrl <> ""

    /// Convenience method that reports if text for this work exists. To be used from .cshtml.
    member v.HasText = v.Text.IsSome

    /// Convenience method that reports if slides for this work exists. To be used from .cshtml.
    member v.HasSlides = v.Slides.IsSome

    /// Convenience method that reports if advisor review for this work exists. To be used from .cshtml.
    member v.HasAdvisorReview = v.AdvisorReview.IsSome

    /// Convenience method that reports if reviewer review for this work exists. To be used from .cshtml.
    member v.HasReviewerReview = v.ReviewerReview.IsSome

    /// Convenience method that reports if commiter account name (for example, GitHub account) for this work is known. 
    /// To be used from .cshtml to generate sources link info.
    member v.HasCommitterName = v.CommitterName <> ""

    /// Adds a new document to the diploma entry, updates metainformation if needed.
    member this.Add (document: Document) =
        if this.Group = "" then
            this.Group <- document.Group
            this.Course <- int (this.Group.Substring(0, 1))

        match document.Kind with
        | Text -> this.Text <- Some document
        | Slides -> this.Slides <- Some document
        | AdvisorReview -> this.AdvisorReview <- Some document
        | ReviewerReview -> this.ReviewerReview <- Some document

    /// Merges metainformation from other diploma to this one, marking this as manually edited.
    /// No documents are merged.
    member this.Merge (other: Diploma) =
        assert (other.ShortName = this.ShortName)
        if other.HasTitle then 
            this.Title <- other.Title
        if other.HasAuthorName then
            this.AuthorName <- other.AuthorName
        if other.HasAdvisorName then
            this.AdvisorName <- other.AdvisorName
        if other.HasConsultantName then
            this.ConsultantName <- other.ConsultantName
        if other.Group <> "" then
            this.Group <- other.Group
        if other.Course <> 0 then
            this.Course <- other.Course
        if other.HasSourcesUrl then
            this.SourcesUrl <- other.SourcesUrl
        if other.HasCommitterName then
            this.CommitterName <- other.CommitterName
        this.ManuallyEdited <- true
