using System.CommandLine;

internal class Context
{
    internal static Context Build(ParseResult parseResult)
    {
        string startUp = parseResult.GetResult("--startup-path")?.GetRequiredValue<string>("--startup-path")!;

        var context = new Context(startUp);
        context.IsVerbose = parseResult.GetResult("--verbose")?.GetValue<bool?>("--verbose") ?? false;

        context.IsRecursive = parseResult.GetResult("--recursive")?.GetValue<bool?>("--recursive") ?? false;

        context.IncludeDotVs = parseResult.GetResult("--include-vs")?.GetValue<bool?>("--include-vs") ?? false;

        context.IsDryRun = parseResult.GetResult("--dryrun")?.GetValue<bool?>("--dryrun") ?? false;
        context.SoftDeletePath = parseResult.GetResult("--softdelete")?.GetValue<string?>("--softdelete");

        context.ShowUI = parseResult.GetResult("--userinterface")?.GetValue<bool?>("--userinterface") ?? false;

        context.LogFilePath = parseResult.GetResult("--log-file")?.GetValue<string?>("--log-file");

        if (context.SoftDelete)
        {
            context.SoftDeletePath = Path.Combine(context.StartPath!, context.SoftDeletePath!);
            context.SoftDeletePath = new DirectoryInfo(context.SoftDeletePath).FullName;
        }
        return context;
    }

    internal Context(string startPath)
    {
        StartPath = startPath;
        if (!string.IsNullOrWhiteSpace(SoftDeletePath))
        {
            SoftDeletePath = Path.Combine(StartPath!, SoftDeletePath);//.Replace("\\.\\", "\\");
        }
    }

    internal string StartPath { get; set; }
    internal string? LogFilePath { get; set; }
    internal bool IsRecursive { get; set; } = false;
    internal bool IncludeDotVs { get; private set; }
    internal bool IsVerbose { get; set; } = false;
    internal bool SoftDelete { 
        get => !string.IsNullOrWhiteSpace(SoftDeletePath);
    }
    internal string? SoftDeletePath { get; set; }
    public bool ShowUI { get; set; }
    internal bool IsDryRun { get; set; } = false;
    internal bool IsForceDelete { get; set; } = false;

    internal ProcessingMode Mode { 
        get
        {
            if (SoftDelete)
            {
                return ProcessingMode.Moving;
            }
            return ProcessingMode.Deleting;
        }
    }

    internal string[] ToStrings()
    {
        List<string> strs = [];
        strs.Add($"Start location   : {StartPath}");
        strs.Add($"Include .vs      : {(IncludeDotVs ? "ON" : "OFF")}");
        strs.Add($"Dry run          : {(IsDryRun ? "ON" : "OFF")}");
        strs.Add($"Soft delete      : {(SoftDelete ? "ON" : "OFF")}");
        strs.Add($"Soft delete path : {SoftDeletePath}");
        strs.Add($"Verbose          : {(IsVerbose ? "ON" : "OFF")}");
        strs.Add($"Recursive        : {(IsRecursive ? "recursively" : "non-recursively")}");
        strs.Add($"Log file path    : {LogFilePath ?? "N/A"}");
        return [.. strs];
    }
}