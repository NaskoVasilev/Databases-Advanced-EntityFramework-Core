using AutoMapper;
using EmployeesSystem.Core.Commands.Contracts;
using EmployeesSystem.Core.ViewModels;
using EmployeesSystem.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Text;

namespace EmployeesSystem.Core.Commands
{
    public class ListEmployeesOlderThanCommand : ICommand
    {
        private Mapper mapper;
        private EmployeeSystemDbContext context;

        public ListEmployeesOlderThanCommand(Mapper mapper, EmployeeSystemDbContext context)
        {
            this.mapper = mapper;
            this.context = context;
        }

        public string Execute(string[] commandParmas)
        {
            int age = int.Parse(commandParmas[0]);

            var employees = context.Employees
                .Include(e => e.Manager)
                .Where(e => e.Birthday.Value.AddYears(age) < DateTime.Now)
                .OrderByDescending(e => e.Salary)
                .Select(e => mapper.CreateMappedObject<EmployeeManagerDto>(e))
                .ToList();

            StringBuilder sb = new StringBuilder();

            foreach (var employee in employees)
            {
                string managerName = employee.Manager != null ? 
                    employee.Manager.FirstName + ' ' + employee.Manager.LastName : "[no manger]";

                sb.AppendLine($"{employee.FirstName} {employee.LastName} - ${employee.Salary} - Manager: {managerName}");
            }

            return sb.ToString().TrimEnd();
        }
    }
}
