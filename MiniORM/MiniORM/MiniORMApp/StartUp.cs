using MiniORMApp.Data;
using MiniORMApp.Data.Entities;
using System;
using System.Linq;

namespace MiniORMApp
{
    class StartUp
    {
        static void Main(string[] args)
        {
            string connectionString = @"Server=DESKTOP-07K6OOE\SQLEXPRESS;Database=MiniORM;Integrated Security=True";

            SoftUniDbContext context = new SoftUniDbContext(connectionString);

            Console.WriteLine("Get initial Employees");

            foreach (var employee in context.Employees)
            {
                Console.WriteLine($"{employee.Id} {employee.FirstName} {employee.LastName} form {employee.Department.Name}");
            }

            context.Employees.Add(new Employee()
            {
                FirstName = "Nasko",
                MiddleName = "Biserov",
                LastName = "Vasilev",
                DepartmentId = context.Departments.First().Id,
                IsEmployed = true
            });


            Employee lastEmployee = context.Employees.Last();
            lastEmployee.FirstName = "Nasko";

            context.SaveChanges();

            Console.WriteLine("Employees after change");
            foreach (var employee in context.Employees)
            {
                Console.WriteLine($"{employee.FirstName} {employee.LastName}");
            }
        }
    }
}
