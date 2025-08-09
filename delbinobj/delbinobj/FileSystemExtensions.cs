public static class FileSystemExtensions
{
    public static void MergeTo(this DirectoryInfo source, DirectoryInfo destination, bool overwrite = true)
    {
        if (source == null) throw new ArgumentNullException(nameof(source));
        if (destination == null) throw new ArgumentNullException(nameof(destination));
        if (!source.Exists)
        {
            throw new DirectoryNotFoundException($"Source directory '{source.FullName}' does not exist.");
        }
        if (!destination.Exists)
        {
            destination.Create();
        }
        foreach (var file in source.GetFiles())
        {
            string destFilePath = Path.Combine(destination.FullName, file.Name);
            file.MoveTo(destFilePath, overwrite);
        }
        foreach (var subDir in source.GetDirectories())
        {
            string destSubDirPath = Path.Combine(destination.FullName, subDir.Name);
            subDir.MergeTo(new DirectoryInfo(destSubDirPath), overwrite);
        }
    }
}
