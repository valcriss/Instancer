using CommandLine;
using instancer.cli.Commands;

namespace instancer.cli;

public static class Program
{
    public static int Main(string[] args)
    {
        return Parser.Default
            .ParseArguments<UpOptions, DownOptions>(args)
            .MapResult(
                (UpOptions opts) => { UpCommand.Run(opts); return 0; },
                (DownOptions opts) => { DownCommand.Run(opts); return 0; },
                errs => 1);
    }
}
