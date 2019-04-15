using AutoMapper;
using EmployeesSystem.Core.Commands.Contracts;
using EmployeesSystem.Core.ViewModels;
using EmployeesSystem.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace EmployeesSystem.Core.Commands
{
    public class EmployeeInfoCommand : ICommand
    {
        private Mapper mapper;
        private EmployeeSystemDbContext context;

        public EmployeeInfoCommand(Mapper mapper, EmployeeSystemDbContext context)
        {
            this.mapper = mapper;
            this.context = context;
        }

        public string Execute(string[] commandParmas)
        {
            int employeeId = int.Parse(commandParmas[0]);
            var employee = context.Employees.Find(employeeId);

            if(employee == null)
            {
                return "There is no employee with Id: " + employeeId;
            }

            EmployeeDto employeeDto = mapper.CreateMappedObject<EmployeeDto>(employee);
            string result = $"ID: {employeeDto.Id} - {employeeDto.FirstName} {employeeDto.LastName} -  ${employeeDto.Salary:F2}";
            return result;
        }
    }
}
