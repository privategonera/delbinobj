internal class Langer
{
    private readonly Context _ctx;
    internal Langer(Context context)
    {
        _ctx = context ?? throw new ArgumentNullException(nameof(context), "Context cannot be null.");
    }
    internal static Langer Build(Context context)
    {
        if (context == null) throw new ArgumentNullException(nameof(context), "Context cannot be null.");
        return new Langer(context);
    }

    internal string Processing => _ctx.Mode switch
    {
        ProcessingMode.Deleting => "Deleting",
        ProcessingMode.Moving => "Moving",
        _ => throw new InvalidOperationException("Unknown processing mode.")
    };

    internal string Process => _ctx.Mode switch
    {
        ProcessingMode.Deleting => "delete",
        ProcessingMode.Moving => "move",
        _ => throw new InvalidOperationException("Unknown processing mode.")
    };

    internal string Processed => _ctx.Mode switch
    {
        ProcessingMode.Deleting => "deleted",
        ProcessingMode.Moving => "moved",
        _ => throw new InvalidOperationException("Unknown processing mode.")
    };

}
