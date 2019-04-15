using EmployeesSystem.Core.Commands.Contracts;
using EmployeesSystem.Data;
using System.Linq;

namespace EmployeesSystem.Core.Commands
{
    public class SetAddressCommand : ICommand
    {
        private EmployeeSystemDbContext context;

        public SetAddressCommand(EmployeeSystemDbContext context)
        {
            this.context = context;
        }

        public string Execute(string[] commandParmas)
        {
            int employeeId = int.Parse(commandParmas[0]);
            string address = commandParmas[1];
            var employee = context.Employees.FirstOrDefault(e => e.Id == employeeId);

            if (employee == null)
            {
                return "There is no employee with Id: " + employeeId;
            }

            employee.Address = address;
            context.SaveChanges();

            string result = $"{employee.FirstName} {employee.LastName} address was set to: {employee.Address}";
            return result;
        }
    }
}
