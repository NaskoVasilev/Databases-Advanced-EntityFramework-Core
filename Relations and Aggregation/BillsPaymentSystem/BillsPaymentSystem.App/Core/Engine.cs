using BillsPaymentSystem.App.Core.Contracts;
using BillsPaymentSystem.Data;
using System;

namespace BillsPaymentSystem.App.Core
{
    public class Engine : IEngine
    {
        private ICommandInterpreter commandInterpreter;
        private BillsPaymentSystemContext context;

        public Engine(ICommandInterpreter commandInterpreter, BillsPaymentSystemContext context)
        {
            this.commandInterpreter = commandInterpreter;
            this.context = context;
        }

        public void Run()
        {
            string input = "";

            while ((input = Console.ReadLine()) != "end")
            {
                string[] data = input.Split(' ');

                string result = commandInterpreter.ParseCommand(data, context);
                Console.WriteLine(result);
            }

        }
    }
}
