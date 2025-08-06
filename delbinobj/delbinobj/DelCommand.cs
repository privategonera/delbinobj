internal class DelCommand(Context _ctx, string _path, Logger _log) : ICommand
{
    public CommandResult Execute()
    {
        try
        {
            _log.Log($"DEL {_path}");
            if (!_ctx.IsDryRun)
            {
                //Directory.Delete(_path);
            }
            return CommandResult.OK;
        }
        catch (Exception ex)
        {
            _log.LogError($"Error deleting directory '{_path}': {ex.Message}");
            return CommandResult.Error;
        }
    }
}