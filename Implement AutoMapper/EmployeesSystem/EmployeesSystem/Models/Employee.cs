using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace EmployeesSystem.Models
{
    public class Employee
    {
        public Employee()
        {
            this.ManagedEmployees = new List<Employee>();
        }

        public int Id { get; set; }

        [Required]
        [StringLength(30, MinimumLength = 3)]
        public string  FirstName { get; set; }

        [Required]
        [StringLength(30, MinimumLength = 3)]
        public string LastName { get; set; }

        [Range(typeof(decimal), "0", "1000000")]
        public decimal Salary { get; set; }

        public DateTime? Birthday { get; set; }

        public string Address { get; set; }

        public int? ManagerId { get; set; }
        public Employee Manager { get; set; }

        public List<Employee> ManagedEmployees { get; set; }
    }
}
