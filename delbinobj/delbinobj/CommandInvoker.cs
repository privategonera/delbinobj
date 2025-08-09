internal class CommandInvoker
{
    private readonly Context _ctx;
    private readonly Logger _log;
    internal CommandInvoker(Context context, Logger logger)
    {
        _ctx = context ?? throw new ArgumentNullException(nameof(context), "Context cannot be null.");
        _log = logger ?? throw new ArgumentNullException(nameof(logger), "Logger cannot be null.");
    }
    internal CommandResult Execute(string path)
    {
        Stats stats = Computer.DirectoryStats(path);

        ICommand? command = _ctx.SoftDelete 
            ? new MoveCommand(_ctx, path, _log) 
            : new DelCommand(_ctx, path, _log);
        
        var cmdRslt = command.Execute();
        cmdRslt.Statistics = stats;
        if (!cmdRslt.Success)
        {
            _log.LogError($"Error during command execution: {cmdRslt.Message}");
            return cmdRslt;
        }
        return cmdRslt;
    }
}