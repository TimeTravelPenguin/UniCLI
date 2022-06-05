module UniCli.Program

open System
open System.IO
open System.Reflection
open Argu
open UniCLI.CommandLine

let parser =
    let errorHandler =
        ProcessExiter(
            colorizer =
                function
                | ErrorCode.HelpText -> Some ConsoleColor.DarkYellow
                | _ -> Some ConsoleColor.Red
        )

    ArgumentParser.Create<CliArguments>(programName = "UniCli.exe", errorHandler = errorHandler)

let appSettingPath =
    let root =
        Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)

    Path.Combine(root, "appSettings.config")

let appSettingExist =
    File.Exists(appSettingPath)

let createAppSettings () =
    printfn
        $"Created `appSettings.config`.{Environment.NewLine}Please edit to template root value before rerunning the application."

    let contents =
        parser.PrintAppSettingsArguments [ TemplatesRoot "path/to/templates/here" ]

    File.WriteAllText(appSettingPath, contents)

let exec (operation: ProjectOperation) (kind: ProjectKind) (templatesPath: string) (path: string) =
    // TODO
    ()

let parse (results: ParseResults<CliArguments>) =
    let path = results.GetResult Path

    let op =
        match results.TryGetResult ProjectOperation with
        | Some operation -> operation
        | None -> ProjectOperation.Create

    let projKind =
        match results.TryGetResult ProjectTemplate with
        | Some kind -> kind
        | None -> ProjectKind.LatexAssignment

    let templatesRoot =
        results.GetResult TemplatesRoot

    exec op projKind templatesRoot path


[<EntryPoint>]
let main args =
    if not appSettingExist then
        createAppSettings ()
    else
        try
            let reader =
                ConfigurationReader.FromAppSettingsFile(appSettingPath)

            let results =
                parser.Parse(inputs = args, raiseOnUsage = true, configurationReader = reader)

            parse results |> ignore
        with
        | e -> printfn $"{e.Message}"

    0
