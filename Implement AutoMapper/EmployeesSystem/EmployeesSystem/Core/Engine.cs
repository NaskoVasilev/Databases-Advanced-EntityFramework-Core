using EmployeesSystem.Core.Contracts;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace EmployeesSystem.Core
{
    public class Engine : IEngine
    {
        private IServiceProvider provider;

        public Engine(IServiceProvider provider)
        {
            this.provider = provider;
        }

        public void Run()
        {
            string input = "";

            while ((input = Console.ReadLine()) != "Exit")
            {
                string[] commandParams = input.Split();
                ICommandInterpreter commandInterpreter = provider.GetService<ICommandInterpreter>();
                string result = commandInterpreter.Read(commandParams);
                Console.WriteLine(result);
            }
        }
    }
}
