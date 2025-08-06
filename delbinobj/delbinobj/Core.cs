internal class Core
{
	Context _ctx;
    Langer _lng;
    Logger _log;
    internal Core(Context context, Langer langer, Logger logger)
	{
        _ctx = context ?? throw new ArgumentNullException(nameof(context), "Context cannot be null."); ;
        _lng = langer ?? throw new ArgumentNullException(nameof(langer), "Langer cannot be null.");
        _log = logger ?? throw new ArgumentNullException(nameof(logger), "Logger cannot be null.");
    }

    internal Result Validate()
    {
        if (string.IsNullOrWhiteSpace(_ctx.StartPath))
		{
            return Result.Error(
                "Startup directory attribute is required.",
                Consts.ExitCodes.ERR_STARTUPDIRREQUIRED
            );
		}

		if (!Directory.Exists(_ctx.StartPath))
		{
            return Result.Error(
                $"Startup directory '{_ctx.StartPath}' does not exist.",
                Consts.ExitCodes.ERR_DIR_NOT_FOUND
            );
		}
		
        return Result.OK;
    }

    internal Result Ensure()
	{
        _log.Log("Ensuring context...");
        if (_ctx.Mode == ProcessingMode.Moving)
        {
            if (!Directory.Exists(_ctx.SoftDeletePath))
            {
                try
                {
                    if (_ctx.IsDryRun)
                    {
                        _log.Log($"(Dry Run) Soft delete directory would be created at '{_ctx.SoftDelete}'.");
                        return Result.OK;
                    }
                    Directory.CreateDirectory(_ctx.SoftDeletePath!);
                    _log.Log($"Soft delete directory created at '{_ctx.SoftDelete}'.");
                }
                catch (Exception ex)
                {
                    return Result.Error(
                        $"Failed to create soft delete directory '{_ctx.SoftDelete}': {ex.Message}",
                        Consts.ExitCodes.ERR_DIR_NOT_FOUND
                    );
                }
            }
            else
            {
                _log.Log($"Soft delete directory already exists at '{_ctx.SoftDelete}'.");
            }
        }
        return Result.OK;
    }

    internal Result Run()
	{
        var di = new DirectoryInfo(_ctx.StartPath!);
        _log.Log($"{(_ctx.IsRecursive ? "Recursively" : "Non-recursively")} scanning directory '{di.FullName}'...");

        var binPaths = ScanFor(di.FullName, "bin", _ctx.IsRecursive, ["*.csproj"]);
        var objPaths = ScanFor(di.FullName, "obj", _ctx.IsRecursive, ["*.csproj"]);
        var vsPaths = _ctx.IncludeDotVs ? ScanFor(di.FullName, ".vs", _ctx.IsRecursive, ["*.csproj", "*.sln"]) : [];

        string[] allPaths = [.. binPaths, .. objPaths, .. vsPaths];
        if (allPaths.Length == 0)
        {
            if (_ctx.IncludeDotVs)
            {
                _log.Log("No bin, obj or .vs directories found.");
            }
            else
            {
                _log.Log("No bin or obj directories found.");
            }
            return new Result(true, Consts.ExitCodes.SUCCESS, "No bin or obj directories found.");
        }
        _log.Log($"Found {allPaths.Length} directories to process.");

        var procRslt = ProcessDirectories(allPaths);
        if (!procRslt.Success)
        {
            _log.LogError($"Processing directories failed: {procRslt.Message}");
            return new Result(false, Consts.ExitCodes.ERR_DELETE_FAILED, procRslt.Message);
        }

        return Result.OK;
    }

    string[] ScanFor(string startPath, string directoryName, bool recursive, string[] accompanyingFileMasks)
    {
        _log.Log($"Scanning for '{directoryName}' directories in '{startPath}'");

        if (!Directory.Exists(startPath))
            return [];

        var searchOption = recursive ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly;
        string[] dirPaths = Directory.GetDirectories(startPath, directoryName, searchOption);

        var filteredPaths = new List<string>(dirPaths.Length);

        foreach (var dirPath in dirPaths)
        {
            bool exists = false;

            foreach (var mask in accompanyingFileMasks)
            {
                if (TryGetNonEmptyFile(dirPath, mask))
                {
                    exists = true;
                    break;
                }
            }

            if (!exists)
            {
                var parentDir = Path.GetDirectoryName(dirPath);
                if (parentDir != null)
                {
                    foreach (var mask in accompanyingFileMasks)
                    {
                        if (TryGetNonEmptyFile(parentDir, mask))
                        {
                            exists = true;
                            break;
                        }
                    }
                }
            }

            if (exists)
            {
                filteredPaths.Add(dirPath);
            }
        }

        _log.Log($"Found {filteredPaths.Count} {directoryName} directories.");
        return [.. filteredPaths];
    }

    private static bool TryGetNonEmptyFile(string path, string searchPattern)
    {
        using var enumerator = Directory.EnumerateFiles(path, searchPattern, SearchOption.TopDirectoryOnly).GetEnumerator();
        return enumerator.MoveNext();
    }

    Result ProcessDirectories(string[] directories)
    {
        string processingStr = _lng.Processing;
        string processStr = _lng.Process;
        string processedStr = _lng.Processed;

        var dirCount = directories.Length;

        _log.Log($"{processingStr} {dirCount} directories...");
        if (_ctx.IsDryRun)
        {
            _log.Log($"DRY RUN - Directories WILL NOT BE {processedStr}");
        }
        directories = [.. directories.Order()];

        CommandInvoker invoker = new CommandInvoker(_ctx, _log);

        Stats stats = Stats.Empty;

        foreach (var dir in directories)
        {
            try
            {
                var cmdRslt = invoker.Execute(dir);
                if (!cmdRslt.Success)
                {
                    _log.LogError($"Failed to {processStr} directory '{dir}'.");
                    return new Result(false, Consts.ExitCodes.ERR_DELETE_FAILED);
                }
                stats.Add(cmdRslt.Statistics);
            }
            catch (Exception ex)
            {
                _log.LogError($"Failed to {processStr} directory '{dir}': {ex.Message}");
                return new Result(false, Consts.ExitCodes.ERR_DELETE_FAILED, ex.Message);
            }
        }
        _log.Log($"{dirCount + stats.SubFolders} directories " +
            $"with {stats.Files} files " +
            $"({Formatter.FormatBytes(stats.Bytes)}) " +
            $"have been {processedStr}.");
        return Result.OK;
    }
}