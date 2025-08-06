internal class Result
{
    internal static Result OK { get; } = new Result();
    internal static Result Error(string message, int exitCode = Consts.ExitCodes.ERR_UNKNOWN) 
        => new Result(false, exitCode, message);

    internal Result(bool success = true, int? exitCode = null, string? message = null)
    {
        Success = success;
        ExitCode = exitCode;
        Message = message;
    }

    internal bool Success { get; }
    internal int? ExitCode { get; }
    internal string? Message { get; }
}
