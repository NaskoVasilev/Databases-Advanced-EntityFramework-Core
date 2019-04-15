using EmployeesSystem.Core.Commands.Contracts;
using EmployeesSystem.Core.Contracts;
using System;
using System.Linq;
using System.Reflection;

namespace EmployeesSystem.Core
{
    class CommandInterpreter : ICommandInterpreter
    {
        private const string Suffix = "Command";
        private IServiceProvider provider;

        public CommandInterpreter(IServiceProvider provider)
        {
            this.provider = provider;
        }

        public string Read(string[] commandParams)
        {
            string commandName = commandParams[0] + Suffix;
            string[] commandArgs = commandParams.Skip(1).ToArray();

            Type commandType = Assembly.GetCallingAssembly()
                .GetTypes()
                .FirstOrDefault(x => x.Name == commandName);

            if(commandType == null)
            {
                return "Invalid command!";
            }

            ConstructorInfo constructor = commandType.GetConstructors().FirstOrDefault();
            object[] constructorParams = constructor.GetParameters()
                .Select(p => provider.GetService(p.ParameterType))
                .ToArray();

            var command = (ICommand)constructor.Invoke(constructorParams);
            string result = command.Execute(commandArgs);
            return result;
        }
    }
}
