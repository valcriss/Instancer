using CommandLine;

namespace instancer.cli.Commands;

[Verb("up", HelpText = "Starts the stack.")]
public class UpOptions
{
    [Option('f', "file", HelpText = "Stack configuration file.")]
    public string? File { get; set; }

    [Option('n', "name", HelpText = "Name of the stack.")]
    public string? Name { get; set; }

    [Option('v', "verbose", HelpText = "Enable verbose output.")]
    public bool Verbose { get; set; }
}

public static class UpCommand
{
    public static void Run(UpOptions opts)
    {
        Console.WriteLine("Executing 'up' command...");
        if (!string.IsNullOrEmpty(opts.File))
        {
            Console.WriteLine($"File: {opts.File}");
        }
        if (!string.IsNullOrEmpty(opts.Name))
        {
            Console.WriteLine($"Name of the stack : {opts.Name}");
        }
        if (opts.Verbose)
        {
            Console.WriteLine("Verbose mode enabled.");
        }
    }
}
