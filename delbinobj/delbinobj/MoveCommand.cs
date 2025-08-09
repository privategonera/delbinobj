using System.Security.Cryptography.X509Certificates;

internal class MoveCommand(Context _ctx, string _path, Logger _log) : ICommand
{
    public CommandResult Execute()
    {
        var srcDir = new DirectoryInfo(_path);
        DirectoryInfo destDir = new DirectoryInfo(_path.Replace(_ctx.StartPath, _ctx.SoftDeletePath!));

        _log.Log($"MOVE {srcDir.FullName} -> {destDir.FullName}");

        if (!_ctx.IsDryRun)
        {
            try
            {
                srcDir.MergeTo(destDir, overwrite: true);
            }
            catch (Exception ex)
            {
                return CommandResult.Error(ex.Message);
            }
        }
        return CommandResult.OK;
    }
}
