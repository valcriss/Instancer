using instancer.cli.Commands.Core;

namespace instancer.cli;

static class Program
{
    static int Main(string[] args)
    {
        CommandManager commandManager = new CommandManager();
        return commandManager.Execute(args);
    }
}