using BillsPaymentSystem.Data;

namespace BillsPaymentSystem.App.Core.Contracts
{
    public interface ICommandInterpreter
    {
        string ParseCommand(string[] data, BillsPaymentSystemContext context);
    }
}
