internal class Logger
{
    public static Logger Build(Context context)
    {
        List<TextWriter> writers = [];
        if (context.IsVerbose)
        {
            writers.Add(Console.Out);
        }
        if (!string.IsNullOrWhiteSpace(context.LogFilePath))
        {
            TextWriter? fileWriter = GetFileWriter(context.LogFilePath);
            if (fileWriter != null)
            {
                writers.Add(fileWriter);
            }
        }
        if (writers.Count == 0)
        {
            //silent mode, warn or not?
        }

        return new Logger([.. writers]);
    }

    private static TextWriter? GetFileWriter(string logFilePath)
    {
        if (string.IsNullOrWhiteSpace(logFilePath))
        {
            Console.Error.WriteLine("Log file path is null or empty.");
            return null;
        }
        try
        {
            var fileWriter = new StreamWriter(logFilePath, append: true);
            fileWriter.AutoFlush = true;
            return fileWriter;
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"Failed to create log file writer: {ex.Message}");
            return null;
        }
    }


    TextWriter[] _writers { get; }
    string _logFormat = "{0:"+Consts.FORMAT_TIMESTAMP+"}: {1}";
    internal Logger()
    {
        _writers = new TextWriter[] { Console.Out };
    }

    internal Logger(params TextWriter[] writers)
    {
        _writers = writers.Length > 0 ? writers : new TextWriter[] { Console.Out };
    }

    internal Logger(string logFilePath)
    {
        if (string.IsNullOrWhiteSpace(logFilePath))
        {
            _writers = [Console.Out];
        }
        else
        {
            var fileWriter = Logger.GetFileWriter(logFilePath);
            if (fileWriter == null)
            {
                return;
            }
        }
    }


    internal void Log(string message)
    {
        string formattedMessage = string.Format(_logFormat, DateTime.Now, message);
        foreach (var writer in _writers)
        {
            if (object.ReferenceEquals(writer, Console.Out))
            {
                // For console output, fill the buffer to the end of the line
                Console.SetCursorPosition(0, Console.CursorTop);
                Console.Write(new string(' ', Console.BufferWidth - 1)); // Clear the line
                Console.SetCursorPosition(0, Console.CursorTop); // Reset cursor position
                Console.WriteLine(formattedMessage);
            }
            else
            {
                // For file or other writers, just write the message
                writer.WriteLine(formattedMessage);
            }
        }
    }

    internal void LogError(string message)
    {
        string msg = $"ERROR: {message}";
        Log(msg);
    }

    internal void LogWarning(string message)
    {
        string msg = $"WARNING: {message}";
        Log(msg);
    }
}