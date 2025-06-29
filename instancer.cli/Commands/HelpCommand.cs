using instancer.cli.Commands.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace instancer.cli.Commands
{
    public class HelpCommand : Command
    {
        public override int Execute(Dictionary<string, string>? args)
        {   
            Console.WriteLine("Available commands:");
            Console.WriteLine("  up       - Starts the stack. Aliases: start, run");
            Console.WriteLine("  down     - Stops the stack. Aliases: stop");
            Console.WriteLine("  help     - Displays this help message.");
            return 0;
        }
    }
}
