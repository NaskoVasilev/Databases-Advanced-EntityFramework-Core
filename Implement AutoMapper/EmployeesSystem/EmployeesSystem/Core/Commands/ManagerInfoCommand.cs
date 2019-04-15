using AutoMapper;
using EmployeesSystem.Core.Commands.Contracts;
using EmployeesSystem.Core.ViewModels;
using EmployeesSystem.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Text;

namespace EmployeesSystem.Core.Commands
{
    public class ManagerInfoCommand : ICommand
    {
        private Mapper mapper;
        private EmployeeSystemDbContext context;

        public ManagerInfoCommand(Mapper mapper, EmployeeSystemDbContext context)
        {
            this.mapper = mapper;
            this.context = context;
        }

        public string Execute(string[] commandParmas)
        {
            int employeeId = int.Parse(commandParmas[0]);
            var employee = context.Employees
                .Include(e => e.ManagedEmployees)
                .FirstOrDefault(e => e.Id == employeeId);

            if(employee == null)
            {
                return "There is no emplooyee with Id: " + employeeId;
            }

            ManagerDto managerDto = mapper.CreateMappedObject<ManagerDto>(employee);

            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"{managerDto.FirstName} {managerDto.LastName} | Employees: {managerDto.ManagedEmployees.Count}");

            foreach (var managedEmpoyee in managerDto.ManagedEmployees)
            {
                sb.AppendLine($"- {managedEmpoyee.FirstName} {managedEmpoyee.LastName} - ${managedEmpoyee.Salary:F2}");
            }

            return sb.ToString().TrimEnd();
        }
    }
}
