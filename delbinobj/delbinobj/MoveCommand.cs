internal class MoveCommand(Context _ctx, string _path, Logger _log) : ICommand
{
    public CommandResult Execute()
    {
        string srcPath = _path;
        string destPath = _path.Replace(_ctx.StartPath, _ctx.SoftDeletePath!);
        try
        {
            _log.Log($"MOVE {srcPath} -> {destPath}");
            if (!_ctx.IsDryRun)
            {
                _log.Log($"MOVE {srcPath} -> {destPath}");
                //Directory.Move(srcPath, destPath);
            }
            return CommandResult.OK;
        }
        catch (Exception ex)
        {
            _log.LogError($"Error moving directory '{srcPath}' to '{destPath}': {ex.Message}");
            return CommandResult.Error;
        }
    }
}
