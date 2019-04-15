using Newtonsoft.Json;
using System.Collections.Generic;

namespace CSharpDataContractJsonSerializer
{
    public class Student
    {
        public Student()
        {
            this.Grades = new List<int>();
        }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public int Age { get; set; }

        public int Class { get; set; }

        public List<int> Grades { get; set; }
    }
}
