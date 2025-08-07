using System;

internal class ProgressBar
{
    public int Value { get; set; } = 0;
    public int Minimum { get; set; } = 0;
    public int Maximum { get; set; } = 100;
    public char ForeChar { get; set; } = '#';
    public ConsoleColor ForeColor { get; set; } = ConsoleColor.Gray;
    public char BackChar { get; set; } = '-';
    public ConsoleColor BackColor { get; set; } = ConsoleColor.DarkGray;
    public int? Width { get; set; }
    public string? TextBefore { get; set; }
    public string? Text { get; set; }
    public string? TextAfter { get; set; }
    public int? X { get; set; }
    public int? Y { get; set; }

    public void Draw()
    {
        string before = TextBefore ?? string.Empty;
        string text = Text ?? string.Empty;
        string after = TextAfter ?? string.Empty;

        int barWidth = Math.Max(1, (Width ?? Console.BufferWidth) - (before.Length + after.Length));
        int range = Maximum - Minimum;
        int value = Math.Clamp(Value, Minimum, Maximum);
        double percent = range > 0 ? (double)(value - Minimum) / range : 0;
        int filled = (int)Math.Round(percent * barWidth);

        int startX = X ?? Console.CursorLeft;
        int startY = Y ?? Console.CursorTop;

        Console.SetCursorPosition(startX, startY);

        // Print TextBefore
        if (before.Length > 0)
            Console.Write(before);

        // Print progress bar (filled + unfilled) in a single buffer for fewer Console.Write calls
        var barBuffer = new char[barWidth];
        if (filled > 0)
            barBuffer.AsSpan(0, filled).Fill(ForeChar);
        if (barWidth - filled > 0)
            barBuffer.AsSpan(filled, barWidth - filled).Fill(BackChar);

        // Write filled part with ForeColor, unfilled with BackColor, minimizing color switches
        if (filled > 0)
        {
            Console.ForegroundColor = ForeColor;
            Console.Write(barBuffer, 0, filled);
        }
        if (barWidth - filled > 0)
        {
            Console.ForegroundColor = BackColor;
            Console.Write(barBuffer, filled, barWidth - filled);
        }
        Console.ResetColor();

        // Print TextAfter
        if (after.Length > 0)
            Console.Write(after);

        // Print Text in the middle of the bar
        if (text.Length > 0 && text.Length <= barWidth)
        {
            int textPosition = startX + before.Length + (barWidth - text.Length) / 2;
            Console.SetCursorPosition(textPosition, startY);
            Console.Write(text);
        }

        // Move cursor to next line
        Console.SetCursorPosition(startX, startY + 1);
    }
}
