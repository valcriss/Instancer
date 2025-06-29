using CommandLine;
using instancer.cli.Commands;

namespace instancer.cli;

public static class Program
{
    private static readonly string[] _commands = ["up", "down", "help"];

    public static int Main(string[] args)
    {
        if (args.Length > 0 && args[0] == "help")
        {
            HelpCommand.Run(new HelpOptions { Command = args.Length > 1 ? args[1] : null });
            return 0;
        }

        if (args.Length > 0 && !args[0].StartsWith("-") && Array.IndexOf(_commands, args[0]) == -1)
        {
            Console.Error.WriteLine($"Unknown command '{args[0]}'.");
            var suggestion = Utils.Suggest(args[0], _commands);
            if (suggestion != null)
            {
                Console.Error.WriteLine($"Did you mean this?\n\t{suggestion}");
            }
            return 1;
        }

        return Parser.Default
            .ParseArguments<UpOptions, DownOptions, HelpOptions>(args)
            .MapResult(
                (UpOptions opts) => { UpCommand.Run(opts); return 0; },
                (DownOptions opts) => { DownCommand.Run(opts); return 0; },
                (HelpOptions opts) => { HelpCommand.Run(opts); return 0; },
                errs => 1);
    }
}
