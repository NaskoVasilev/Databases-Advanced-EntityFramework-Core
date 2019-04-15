using EmployeesSystem.Core.Commands.Contracts;
using EmployeesSystem.Data;

namespace EmployeesSystem.Core.Commands
{
    public class SetManagerCommand : ICommand
    {
        private EmployeeSystemDbContext context;

        public SetManagerCommand(EmployeeSystemDbContext context)
        {
            this.context = context;
        }

        public string Execute(string[] commandParmas)
        {
            int employeeId = int.Parse(commandParmas[0]);
            int managerId = int.Parse(commandParmas[1]);

            var manager = context.Employees.Find(managerId);
            var employee = context.Employees.Find(employeeId);

            if(manager == null)
            {
                return "There is no manager with Id: " + managerId;
            }

            if (employee == null)
            {
                return "There is no employee with Id: " + employeeId;
            }

            employee.Manager = manager;
            context.SaveChanges();

            string result = $"Manager - {manager.FirstName} {manager.LastName} has employee - {employee.FirstName} {employee.LastName}";
            return result;
        }
    }
}
