using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace instancer.cli.Commands.Core
{
    public abstract class CommandParameters
    {
        public abstract bool CheckParameters(string[] args);
    }
}
