using BillsPaymentSystem.App.Core.Commands.Contracts;
using BillsPaymentSystem.App.Core.Contracts;
using BillsPaymentSystem.Data;
using System;
using System.Linq;
using System.Reflection;

namespace BillsPaymentSystem.App.Core
{
    public class CommandInterpreter : ICommandInterpreter
    {
        private const string Suffix = "Command";

        public string ParseCommand(string[] data, BillsPaymentSystemContext context)
        {
            string commandName = data[0] + Suffix;
            string[] commandParams = data.Skip(1).ToArray();
            Type commandType = Assembly.GetCallingAssembly()
                .GetTypes()
                .FirstOrDefault(t => t.Name == commandName);

            if (commandType == null)
            {
                return "This command is not supported!";
            }

            var commandInstance = Activator.CreateInstance(commandType, context);
            ICommand command = (ICommand)commandInstance;
            string result = command.Execute(commandParams);
            return result;
        }
    }
}
