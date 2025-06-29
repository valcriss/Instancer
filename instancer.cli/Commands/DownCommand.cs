using CommandLine;

namespace instancer.cli.Commands;

[Verb("down", HelpText = "Stops the stack.")]
public class DownOptions
{
    [Option('v', "verbose", HelpText = "Enable verbose output.")]
    public bool Verbose { get; set; }
}

public static class DownCommand
{
    public static void Run(DownOptions opts)
    {
        Console.WriteLine("Executing 'down' command...");
        if (opts.Verbose)
        {
            Console.WriteLine("Verbose mode enabled.");
        }
    }
}
