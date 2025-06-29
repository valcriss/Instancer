using CommandLine;

namespace instancer.cli.Commands;

[Verb("help", HelpText = "Show help for commands.")]
public class HelpOptions
{
    [Value(0, MetaName = "command", HelpText = "Command to show help for.")]
    public string? Command { get; set; }
}

public static class HelpCommand
{
    public static void Run(HelpOptions opts)
    {
        if (string.IsNullOrEmpty(opts.Command))
        {
            Parser.Default.ParseArguments<UpOptions, DownOptions, HelpOptions>(new[] { "--help" });
            return;
        }

        var cmd = opts.Command!.ToLowerInvariant();
        switch (cmd)
        {
            case "up":
                Parser.Default.ParseArguments<UpOptions>(new[] { "--help" });
                break;
            case "down":
                Parser.Default.ParseArguments<DownOptions>(new[] { "--help" });
                break;
            case "help":
                Parser.Default.ParseArguments<HelpOptions>(new[] { "--help" });
                break;
            default:
                Console.Error.WriteLine($"Unknown command '{opts.Command}'.");
                var suggestion = Utils.Suggest(cmd, new[] { "up", "down", "help" });
                if (suggestion != null)
                {
                    Console.Error.WriteLine($"Did you mean this?\n\t{suggestion}");
                }
                break;
        }
    }
}
