using System.CommandLine;
using System.CommandLine.Parsing;
using System.Diagnostics;

internal class Application
{
    internal async Task<int> RunAsync(string[] args)
    {
        var rootCommand = BuildRootCommand();
        rootCommand.SetAction(RootHandler);
        return await rootCommand
            .Parse(args, new CommandLineConfiguration(rootCommand))
            .InvokeAsync();
    }

    RootCommand BuildRootCommand()
    {
        var startUpOption = new Option<string>(name: "--startup-path", aliases: ["-p"])
        {
            Description = "Directory where command will look for *.csproj file to locate bin and obj folders to delete",
            DefaultValueFactory = _ => ".",
        };

        var verboseOption = new Option<bool>("--verbose", aliases: ["-v"]) {
            Description = "If present, command will print verbose output to console",
        };

        var recursiveOption = new Option<bool>("--recursive", aliases: ["-r"]) {
            Description = "If present, command will scan startup directory and all directories below for *.csproj files",
        };

        var dotvsDirOption = new Option<bool>("--include-vs", aliases: ["-vs"]) {
            Description = "If present, command will delete .vs directory from solution directory",
        };

        var logFileOption = new Option<string?>("--log-file", aliases: ["-l"]) {
            Description = "If present, command will log output to specified file",
        };

        var softDeletePathOption = new Option<string?>("--softdelete", aliases: ["-sd"]) {
            Description = "If present, the command will move (instead of delete) found directories to the directory specified in this argument, maintaining the relationship to the start-up path",
        };

        var dryRunOption = new Option<bool>("--dryrun", aliases: ["-dr"]) {
            Description = "If present, command will not delete or move anything, just print what would processed",
        };

        var uiOption = new Option<bool>("--userinterface", aliases: ["-ui"]) {
            Description = "If present, progress bar will be presented",
        };

        var queryOption = new Option<bool>("--query", aliases: ["-q"]) {
            Description = "If present, the command will interactively ask for all options",
        };

        return new RootCommand("Deletes bin and obj directories in a .NET project(s)")
        {
            startUpOption,
            recursiveOption,
            dotvsDirOption,
            softDeletePathOption,
            dryRunOption,
            verboseOption,
            uiOption,
            logFileOption,
            queryOption,
        };
    }

    int RootHandler(ParseResult parseResult)
    {
        Stopwatch stopwatch = Stopwatch.StartNew();
        bool query = parseResult.GetResult("--query")?.GetValue<bool?>("--query") ?? false;
        Context context = query ? Context.BuildInteractive() : Context.Build(parseResult);
        Logger log = Logger.Build(context);
        var shell = new Shell(log, context);
        var runRslt = shell.Run();
        stopwatch.Stop();
        log.Log($"Command completed in {stopwatch}");
        return runRslt;
    }
}

