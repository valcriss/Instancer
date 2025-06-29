using instancer.cli.Commands.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace instancer.cli.Commands
{
    public class UpCommand : Command
    {
        public override string Name { get; protected set; } = "up";
        public override string Description { get; protected set; } = "Starts the stack.";
        public override string[]? Aliases { get; protected set; } = new[] { "start", "run" };

        public override int Execute(Dictionary<string, string>? args)
        {
            Console.WriteLine("Executing 'up' command...");
            return 0;
        }
    }
}
