namespace EmployeesSystem.Core.Commands.Contracts
{
    public interface ICommand
    {
        string Execute(string[] commandParmas);
    }
}
