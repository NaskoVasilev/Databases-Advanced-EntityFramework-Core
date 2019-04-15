using System.Collections.Generic;

namespace AutoMapper.App.Models
{
    public class Student
    {
        public string Name { get; set; }

        public int Age { get; set; }

        public int NumberInClass { get; set; }

        public int Class { get; set; }

        public ICollection<StudentCourse> StudentCourses { get; set; } = new List<StudentCourse>();
    }
}
