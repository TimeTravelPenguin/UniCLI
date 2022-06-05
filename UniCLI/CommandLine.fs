module UniCLI.CommandLine

open Argu

type Configuration = { Configs: ConfigKind list }
and ConfigKind = { Name: string; TemplateRoot: string }

type ProjectKind =
    | LatexAssignment
    | CourseDirectory

type ProjectOperation =
    | Create
    | Update

type CliArguments =
    | [<Unique; EqualsAssignment; AltCommandLine("-op")>] ProjectOperation of ProjectOperation
    | [<Mandatory; Last; Unique; EqualsAssignment; AltCommandLine("-p")>] Path of string
    | [<Unique; EqualsAssignment; AltCommandLine("-template")>] ProjectTemplate of ProjectKind
    | [<NoCommandLine>] TemplatesRoot of string

    interface IArgParserTemplate with
        member s.Usage =
            match s with
            | ProjectOperation _ -> "specify whether to create a new project or update an existing one."
            | Path _ -> "specify the path of a new project, or the root of an existing one."
            | ProjectTemplate _ -> "specify the project template to use."
            | TemplatesRoot _ -> "the root directory of all project templates"
