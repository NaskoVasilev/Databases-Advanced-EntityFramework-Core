using AutoMapper;
using EmployeesSystem.Core.Commands.Contracts;
using EmployeesSystem.Data;

namespace EmployeesSystem.Core.Commands
{
    public class EmployeePersonalInfoCommand : ICommand
    {
        private EmployeeSystemDbContext context;

        public EmployeePersonalInfoCommand(Mapper mapper, EmployeeSystemDbContext context)
        {
            this.context = context;
        }

        public string Execute(string[] commandParmas)
        {
            int employeeId = int.Parse(commandParmas[0]);
            var employee = context.Employees.Find(employeeId);

            if (employee == null)
            {
                return "There is no employee with Id: " + employeeId;
            }

            string result = $"ID: {employee.Id} - {employee.FirstName} {employee.LastName} -  ${employee.Salary:F2}\n";
            result += $"Birthday: {employee.Birthday:dd-MM-yyyy}\n";
            result += $"Address: {employee.Address}";
            return result;
        }
    }
}
