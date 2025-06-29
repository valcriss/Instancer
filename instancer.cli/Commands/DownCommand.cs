using instancer.cli.Commands.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace instancer.cli.Commands
{
    public class DownCommand : Command
    {
        public override string Name { get; protected set; } = "down";
        public override string Description { get; protected set; } = "Stops the stack.";
        public override string[]? Aliases { get; protected set; } = new[] { "stop" };

        public override int Execute(Dictionary<string, string>? args)
        {
            Console.WriteLine("Executing 'down' command...");
            return 0;
        }
    }
}
