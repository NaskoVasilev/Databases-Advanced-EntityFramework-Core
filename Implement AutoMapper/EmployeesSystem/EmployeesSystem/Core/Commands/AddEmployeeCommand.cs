using AutoMapper;
using EmployeesSystem.Core.Commands.Contracts;
using EmployeesSystem.Core.ViewModels;
using EmployeesSystem.Data;
using EmployeesSystem.Models;

namespace EmployeesSystem.Core.Commands
{
    public class AddEmployeeCommand : ICommand
    {
        private Mapper mapper;
        private EmployeeSystemDbContext context;

        public AddEmployeeCommand(Mapper mapper, EmployeeSystemDbContext context)
        {
            this.mapper = mapper;
            this.context = context;
        }

        public string Execute(string[] commandParmas)
        {
            string firstName = commandParmas[0];
            string lastName = commandParmas[1];
            decimal salary = decimal.Parse(commandParmas[2]);

            Employee employee = new Employee()
            {
                FirstName = firstName,
                LastName = lastName,
                Salary = salary
            };

            context.Employees.Add(employee);
            context.SaveChanges();

            EmployeeDto employeeDto = mapper.CreateMappedObject<EmployeeDto>(employee);
            string result = $"New Employee created successfully: {employeeDto.FirstName} {employeeDto.LastName} - Salary: {employeeDto.Salary:F2}";
            return result;
        }
    }
}
