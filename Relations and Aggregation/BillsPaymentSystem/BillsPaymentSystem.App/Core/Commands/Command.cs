using BillsPaymentSystem.App.Core.Commands.Contracts;
using BillsPaymentSystem.Data;

namespace BillsPaymentSystem.App.Core.Commands
{
    public abstract class Command : ICommand
    {
        protected BillsPaymentSystemContext Context { get; set; }

        public Command(BillsPaymentSystemContext context)
        {
            this.Context = context;
        }

        public abstract string Execute(string[] data);
    }   
}
