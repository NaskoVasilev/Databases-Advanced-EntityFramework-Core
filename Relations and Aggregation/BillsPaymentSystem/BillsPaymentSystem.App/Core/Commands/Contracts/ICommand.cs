namespace BillsPaymentSystem.App.Core.Commands.Contracts
{
    public interface ICommand
    {
        string Execute(string[] data);
    }
}
