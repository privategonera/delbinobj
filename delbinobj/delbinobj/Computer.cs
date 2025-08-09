internal static class Computer
{
    internal static Stats DirectoryStats(string path)
    {
        if (string.IsNullOrWhiteSpace(path))
        {
            throw new ArgumentNullException(nameof(path), "Path cannot be null or empty.");
        }
        if (!Directory.Exists(path))
        {
            throw new ArgumentException($"Directory '{path}' does not exist.", nameof(path));
        }
        var stats = new Stats();
        try
        {
            var dirInfo = new DirectoryInfo(path);
            stats.SubFolders = dirInfo.GetDirectories("*", SearchOption.AllDirectories).Length;
            var files = dirInfo.GetFiles("*", SearchOption.AllDirectories);
            stats.Files = files.Length;
            stats.Bytes = files.Sum(file => file.Length);
            return stats;
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Error calculating statistics for directory '{path}': {ex.Message}", ex);
        }
    }

    internal static Stats DirectoriesStats(params string[] paths)
    {
        if (paths == null || paths.Length == 0)
        {
            throw new ArgumentNullException(nameof(paths), "Paths cannot be null or empty.");
        }
        var totalStats = new Stats();
        foreach (var path in paths)
        {
            var stats = DirectoryStats(path);
            totalStats.Add(stats);
        }
        return totalStats;
    }
}
