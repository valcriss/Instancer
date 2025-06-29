using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace instancer.cli.Commands.Core
{
    public class CommandManager
    {
        private Dictionary<string, Command> Commands { get; } = new Dictionary<string, Command>(StringComparer.OrdinalIgnoreCase);
        private Command? _helpCommand = null;

        public CommandManager()
        {
            RegisterCommand(new UpCommand());
            RegisterCommand(new DownCommand());
            RegisterCommand(new HelpCommand(), true);
        }

        public void RegisterCommand(Command command, bool isHelpCommand = false)
        {
            if (command == null)
            {
                throw new ArgumentNullException(nameof(command), "Command cannot be null.");
            }
            if (Commands.ContainsKey(command.Name))
            {
                throw new InvalidOperationException($"Command '{command.Name}' is already registered.");
            }
            Commands[command.Name] = command;
            if (command.Aliases != null)
            {
                foreach (var alias in command.Aliases)
                {
                    if (Commands.ContainsKey(alias))
                    {
                        throw new InvalidOperationException($"Alias '{alias}' is already registered for another command.");
                    }
                    Commands[alias] = command;
                }
            }
            if (isHelpCommand)
            {
                if (_helpCommand != null)
                {
                    throw new InvalidOperationException("Only one help command can be registered.");
                }
                _helpCommand = command;
            }
        }

        public int Execute(string[] args)
        {
            if (args != null && args.Length != 0)
            {
                string commandName = args[0];
                if (Commands.TryGetValue(commandName, out Command? command))
                {
                    return command.Execute(GetCommands(args));
                }
            }

            if (_helpCommand != null)
            {
                return _helpCommand.Execute(null);
            }
            Console.WriteLine("No command provided. Use 'help' to see available commands.");
            return 1;
        }

        public Dictionary<string, string> GetCommands(string[] args)
        {
            string[] commandArgs = args.Skip(1).ToArray();
            if (commandArgs.Length == 0) return new Dictionary<string, string>();
            Dictionary<string, string> parameters = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            for (int i = 0; i < commandArgs.Length; i += 2)
            {
                string paramName = commandArgs[i];
                string paramValue = i + 1 < commandArgs.Length ? commandArgs[i + 1] : string.Empty;
                if (parameters.ContainsKey(paramName))
                {
                    parameters[paramName] = paramValue;
                }
                else
                {
                    parameters.Add(paramName, paramValue);
                }
            }
            return parameters;
        }
    }
}
