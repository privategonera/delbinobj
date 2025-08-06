using System;
using System.Collections.Generic;
using System.CommandLine;
using System.CommandLine.Completions;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

internal class DirCompletionSource
{
    internal static IEnumerable<CompletionItem> Complete(CompletionContext cctx)
    {
        if (cctx is null)
        {
            throw new ArgumentNullException(nameof(cctx));
        }
        if (cctx.ParseResult is null)
        {
            throw new ArgumentNullException(nameof(cctx.ParseResult));
        }
        var pathArg = new Argument<string>("--startup-path")
        {
            Arity = ArgumentArity.ExactlyOne
        };
        var startPath = cctx.ParseResult.GetValue(pathArg);
        if (string.IsNullOrWhiteSpace(startPath))
        {
            return [];
        }
        try
        {
            var directories = Directory.GetDirectories($".\\{startPath}");
            return directories.Select(d => new CompletionItem(d));
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"Error retrieving directories: {ex.Message}");
            return [];
        }
    }
}

