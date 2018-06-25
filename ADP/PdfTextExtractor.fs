namespace ADP

/// Some tooling around iText that allows to extract text from typical PDF with qualification work. iText 
/// location-based text extraction strategy messes up spaces in a text and does not allow to configure itself,
/// so it was reimplemented ad-hoc and possibly works completely wrong for some documents. But it does its job.
module PdfTextExtractor = 
    open System.IO
    open iText.Kernel.Pdf
    open iText.Kernel.Geom
    open System
    open iText.Kernel.Pdf.Canvas.Parser
    open iText.Kernel.Pdf.Canvas.Parser.Listener

    /// Own inplementation of ITextChunkLocation interface, because iText does not provide default implementation 
    /// for some reason. Provides storage for coordinates of a PDF text chunk and some utility operations.
    type private TextChunkLocation(x1, y1, x2, y2) =
        override this.ToString () =
            sprintf "(%f, %f) - (%f, %f)" x1 y1 x2 y2

        interface ITextChunkLocation with
            member this.DistParallelEnd() = raise (NotImplementedException())
            member this.DistParallelStart() = raise (NotImplementedException())
            member this.DistPerpendicular() = raise (NotImplementedException())
            member this.GetCharSpaceWidth() = raise (NotImplementedException())
            member this.GetEndLocation() = Vector(x2, y2, 1.0f)
            member this.GetStartLocation() = Vector(x1, y1, 1.0f)
            member this.IsAtWordBoundary _ = raise (NotImplementedException())
            member this.OrientationMagnitude() = raise (NotImplementedException())
            member this.SameLine(``as``: ITextChunkLocation): bool = 
                abs(y1 - ``as``.GetStartLocation().Get(1)) < 0.001f
            member this.DistanceFromEndOf(other: ITextChunkLocation) =
                x1 - other.GetEndLocation().Get(0)

    /// Text extraction strategy that collects all text chunks in a page, sorts them and then merges text from chunks 
    /// that are overlapping or very near to each other (so we believe they are from the same word).
    type private MyTextExtractionStrategy() =
        /// A list of detected chunks of text in a page.
        let mutable chunks: TextChunk list = []

        /// Heuristic constant that denotes a distance between two chunks that allows to assume that there is a space
        /// symbol between them. If two chunks are separated by lesser distance, we assume that they are parts of 
        /// the same word.
        let spaceDistance = 1.0f

        let collector (text, lastChunk: TextChunk) (newChunk: TextChunk) =
            if lastChunk.GetText() = "" then
                (text + newChunk.GetText(), newChunk)
            else
                if not <| newChunk.GetLocation().SameLine(lastChunk.GetLocation()) then
                    (text + "\n" +  newChunk.GetText(), newChunk)
                else
                    if newChunk.GetLocation().DistanceFromEndOf(lastChunk.GetLocation()) < spaceDistance then
                        (text + newChunk.GetText(), newChunk)
                    else
                        (text + " " + newChunk.GetText(), newChunk)

        interface ITextExtractionStrategy with
            member this.EventOccurred(data: Data.IEventData, ``type``: EventType): unit = 
                match ``type`` with
                | EventType.RENDER_TEXT -> 
                    let data = data :?> Data.TextRenderInfo
                    let text = data.GetText()
                    let bottomLeft = data.GetDescentLine().GetStartPoint()
                    let topRight = data.GetAscentLine().GetEndPoint()
                    let chunkLocation = TextChunkLocation(
                                                          bottomLeft.Get(0), 
                                                          bottomLeft.Get(1), 
                                                          topRight.Get(0), 
                                                          topRight.Get(1))
                    let chunk = TextChunk(text, chunkLocation)
                    chunks <- chunk :: chunks
                    ()
                | _ -> failwith "unsupported event"

            member this.GetSupportedEvents(): Collections.Generic.ICollection<EventType> = 
                Collections.Generic.List([EventType.RENDER_TEXT]) :> _

            member this.GetResultantText () =
                chunks <- List.rev chunks
                chunks <- List.sortBy (fun chunk -> 
                    let startLocation = chunk.GetLocation().GetStartLocation()
                    -startLocation.Get(1) * 10000.0f + chunk.GetLocation().GetStartLocation().Get(0)
                    ) chunks
                let startingChunk = TextChunk("", TextChunkLocation(0.0f, 0.0f, 0.0f, 0.0f))
                let result = List.fold collector ("", startingChunk) chunks
                             |> fst
                result

    /// Gets all text on a title page trying to preserve spaces and line breaks.
    let getFirstPageText (file: string) = 
        let fullFileName = Path.Combine(Environment.CurrentDirectory, file)
        use reader = new PdfReader(fullFileName)
        use pdf = new PdfDocument(reader)
        let titlePage = pdf.GetFirstPage()
        let strategy = MyTextExtractionStrategy() :> ITextExtractionStrategy
        let parser = PdfCanvasProcessor(strategy)
        parser.ProcessPageContent titlePage
        let text = strategy.GetResultantText()
        text


