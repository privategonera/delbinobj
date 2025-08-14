internal class Shell
{
    readonly Context _ctx;
    readonly Core _core;
    readonly Logger _log;
    readonly ProgressBar? _bar;
    internal Shell(Logger log, Context context)
    {
        _log = log ?? throw new ArgumentNullException(nameof(log), "Logger cannot be null.");
        _ctx = context ?? throw new ArgumentNullException(nameof(context), "Context cannot be null.");
        
        _bar = _ctx.ShowUI ? new ProgressBar() { X = 0, Y = 0 } : null;

        Langer lng = Langer.Build(_ctx);
        _core = new Core(_ctx, lng, _log);

        if (_ctx.IsVerbose)
        {
            _log.Log("Running with options:");
            _ctx.ToStrings().ToList().ForEach(line => _log.Log($" {line}"));
        }
    }

    internal int Run()
    {
        var valRslt = _core.Validate();
        if (!valRslt.Success)
        {
            _log.LogError($"Validation failed: {valRslt.Message}");
            return valRslt.ExitCode ?? Consts.ExitCodes.ERR_UNKNOWN;
        }

        var ensRslt = _core.Ensure();
        if (!ensRslt.Success)
        {
            _log.LogError($"Ensuring failed: {ensRslt.Message}");
            return ensRslt.ExitCode ?? Consts.ExitCodes.ERR_UNKNOWN;
        }

        var runRslt = _core.Run();
        if (!runRslt.Success)
        {
            _log.LogError($"Run failed: {runRslt.Message}");
            return runRslt.ExitCode ?? Consts.ExitCodes.ERR_UNKNOWN;
        }

        return 0;
    }
}
