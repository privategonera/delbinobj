internal class Program
{
    static async Task<int> Main(string[] args)
    {
        try
        {
            var app = new Application();
            return await app.RunAsync(args);
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"An FATAL error occurred: {ex.Message}");
            return Consts.ExitCodes.ERR_FATAL;
        }
    }
}
