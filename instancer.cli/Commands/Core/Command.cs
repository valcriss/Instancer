using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace instancer.cli.Commands.Core
{
    public abstract class Command
    {
        public virtual string Name { get; protected set; } = string.Empty;
        public virtual string Description { get; protected set; } = string.Empty;
        public virtual string[]? Aliases { get; protected set; } = null;

        public abstract int Execute(Dictionary<string,string>? args);
    }
}
