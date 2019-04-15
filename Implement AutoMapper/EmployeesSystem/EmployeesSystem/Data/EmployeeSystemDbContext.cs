using EmployeesSystem.Models;
using Microsoft.EntityFrameworkCore;

namespace EmployeesSystem.Data
{
    public class EmployeeSystemDbContext : DbContext
    {
        public EmployeeSystemDbContext(DbContextOptions<EmployeeSystemDbContext> options) : base(options)
        {
        }

        public EmployeeSystemDbContext()
        {
        }

        public DbSet<Employee> Employees { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer(@"Server=DESKTOP-07K6OOE\SQLEXPRESS;Database=EmployeesSyetem;Integrated Security=True");
            }

            base.OnConfiguring(optionsBuilder);
        }
    }
}
