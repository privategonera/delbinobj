using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

internal class ProgressBar
{
    public int Value { get; set; } = 0;
    public int Minimum { get; set; } = 0;
    public int Maximum { get; set; } = 100;
    public char ForeChar { get; set; } = '#';
    public ConsoleColor ForeColor { get; set; } = ConsoleColor.Gray;
    public char BackChar { get; set; } = '-';
    public ConsoleColor BackColor { get; set; } = ConsoleColor.DarkGray;
    public int Width { get; set; } = 40;
    public int Height { get; set; } = 1;
    public string? TextBefore { get; set; }
    public string? Text { get; set; }
    public string? TextAfter { get; set; }
    public int? X { get; set; }
    public int? Y { get; set; }

    public void Draw()
    {
        int barWidth = Math.Max(1, Width);
        int barHeight = Math.Max(1, Height);
        int range = Maximum - Minimum;
        int value = Math.Clamp(Value, Minimum, Maximum);
        double percent = range > 0 ? (double)(value - Minimum) / range : 0;
        int filled = (int)Math.Round(percent * barWidth);

        string before = TextBefore ?? "";
        string text = Text ?? "";
        string after = TextAfter ?? "";

        int startX = X ?? Console.CursorLeft;
        int startY = Y ?? Console.CursorTop;

        for (int h = 0; h < barHeight; h++)
        {
            Console.SetCursorPosition(startX, startY + h);

            // Print TextBefore
            if (h == 0 && !string.IsNullOrEmpty(before))
            {
                Console.Write(before);
            }

            // Print progress bar
            Console.ForegroundColor = ForeColor;
            Console.BackgroundColor = ConsoleColor.Black;
            Console.Write(new string(ForeChar, filled));

            Console.ForegroundColor = BackColor;
            Console.BackgroundColor = ConsoleColor.Black;
            Console.Write(new string(BackChar, barWidth - filled));

            Console.ResetColor();

            // Print Text
            if (h == 0 && !string.IsNullOrEmpty(text))
            {
                Console.Write(" " + text);
            }

            // Print TextAfter
            if (h == 0 && !string.IsNullOrEmpty(after))
            {
                Console.Write(" " + after);
            }
        }
    }
}
