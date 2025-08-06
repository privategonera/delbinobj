internal class CommandResult
{
    internal static CommandResult OK => new CommandResult();
    internal static CommandResult Error = new CommandResult(false);
    internal CommandResult(bool success = true)
    {
        Success = success;
        Statistics = Stats.Empty;
    }
    internal bool Success { get; private set; }
    internal Stats Statistics { get; set; }
}
