/// <summary>  
/// Command-line tool that scans a specified 
/// directory for .NET project files (.csproj) and 
/// deletes the associated bin, obj and optionally
/// .vs directories. 
/// </summary>
using System.CommandLine;
using System.Diagnostics;
//using System.CommandLine.Completions;
try
{
    var startUpOption = new Option<string>(name: "--startup-path", ["-p"]);
    startUpOption.DefaultValueFactory = (argRes) => ".";
    startUpOption.Description = "Directory where command will look for *.csproj file to locate bin and obj folders to delete";
    //startUpOption.CompletionSources.Add(DirCompletionSource.Complete); //TODO

    var verboseOption = new Option<bool>("--verbose", ["-v"]);
    verboseOption.Description = "If present, command will print verbose output to console";

    var recursiveOption = new Option<bool>("--recursive", ["-r"]);
    recursiveOption.Description = "If present, command will scan startup directory and all directories below for *.csproj files";

    var dotvsDirOption = new Option<bool>("--include-vs", ["-vs"]);
    dotvsDirOption.Description = "If present, command will delete .vs directory from solution directory";

    var logFileOption = new Option<string?>("--log-file", ["-l"]);
    logFileOption.Description = "If present, command will log output to specified file";

    var softDeletePathOption = new Option<string?>("--softdelete", ["-sd"]);
    softDeletePathOption.Description = "If present, the command will move (instead of delete) found directories to the directory specified in this argument, maintaining the relationship to the start-up path";

    var dryRunOption = new Option<bool>("--dryrun", ["-dr"]);
    dryRunOption.Description = "If present, command will not delete or move anything, just print what would processed";

    var uiOption = new Option<bool>("--userinterface", ["-ui"]);
    uiOption.Description = "If present, progress bar will be presented";

    //var transactionalOption = new Option<bool>("--transactional", ["-t"]);
    //transactionalOption.Description = "If present and soft delete mode is on, command will use transaction to move operation";

    //var anomalyOption = new Option<bool>("--anomaly", ["-a"]);
    //anomalyOption.Description = "If present, command will omit bin and obj directory that looks like they contain something more than compiled sources";

    var rootCommand = new RootCommand("Deletes bin and obj directories in a .NET project(s)")
    {
        startUpOption,
        recursiveOption,
        dotvsDirOption,
        softDeletePathOption,
        dryRunOption,
        verboseOption,
        uiOption,
        logFileOption,
    };

    rootCommand.SetAction(
        (ParseResult parseResult) =>
        {
            Stopwatch stopwatch = Stopwatch.StartNew();

            Context context = Context.Build(parseResult);

            ProgressBar? progressBar = context.ShowUI ? new ProgressBar() : null;

            Logger log = Logger.Build(context);

            var shell = new Shell(log, context);

            var runRslt = shell.Run();

            stopwatch.Stop();
            log.Log($"Command completed in {stopwatch}");
            return runRslt;
        }
    );
    
    var rootRslt = await rootCommand
                    .Parse(args, new CommandLineConfiguration(rootCommand))
                    .InvokeAsync();

    return rootRslt;
}
catch (Exception ex)
{
    Console.Error.WriteLine($"An FATAL error occurred: {ex.Message}");
    return Consts.ExitCodes.ERR_FATAL;
}
finally
{
    Console.WriteLine("Exiting application.");
}