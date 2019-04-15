using EmployeesSystem.Core.Commands.Contracts;
using EmployeesSystem.Core.ViewModels;
using EmployeesSystem.Data;
using System;
using System.Globalization;
using System.Linq;

namespace EmployeesSystem.Core.Commands
{
    public class SetBirthdayCommand : ICommand
    {
        private EmployeeSystemDbContext context;

        public SetBirthdayCommand(EmployeeSystemDbContext context)
        {
            this.context = context;
        }

        public string Execute(string[] commandParmas)
        {
            int employeeId = int.Parse(commandParmas[0]);
            DateTime birthday = DateTime.ParseExact(commandParmas[1], "dd-MM-yyyy", CultureInfo.InvariantCulture);

            var employee = context.Employees.FirstOrDefault(e => e.Id == employeeId);

            if(employee == null)
            {
                return "There is no employee with Id: " + employeeId;
            }

            employee.Birthday = birthday;
            context.SaveChanges();

            string result = $"{employee.FirstName} {employee.LastName} birtdate was set to: " +
                $"{employee.Birthday.Value.ToString("dd-MM-yyyy")}";
            return result;
        }
    }
}
