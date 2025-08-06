
internal class Stats
{
    internal static Stats Empty => new Stats
    {
        SubFolders = 0,
        Files = 0,
        Bytes = 0
    };

    internal int SubFolders { get; set; }
    internal int Files { get; set; }
    internal long Bytes { get; set; }

    internal void Add(Stats statistics)
    {
        if (statistics == null)
        {
            throw new ArgumentNullException(nameof(statistics), "Statistics cannot be null.");
        }
        SubFolders += statistics.SubFolders;
        Files += statistics.Files;
        Bytes += statistics.Bytes;
    }
}
