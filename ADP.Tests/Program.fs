module Program

/// Entry point, added to suppress compiler warning about empty program. Test library is not supposed to be program
/// but it seems that any .NET Core 2.0 assembly referencing .NET Core 2.0 Application shall itself be 
/// .NET Core 2.0 Application and compiler will provide an entry point for it.
[<EntryPoint>]
let main argv =
    printfn "This is a test library which can not be library due to strange .NET Core limitations."
    printfn "Not supposed to be run directly."
    0