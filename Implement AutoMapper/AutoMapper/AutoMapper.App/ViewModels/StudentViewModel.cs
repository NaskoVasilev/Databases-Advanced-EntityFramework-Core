using System.Collections.Generic;

namespace AutoMapper.App.ViewModels
{
    public class StudentViewModel
    {
        public string Name { get; set; }

        public int Age { get; set; }

        public ICollection<StudentCourseViewModel> StudentCourses { get; set; } = new List<StudentCourseViewModel>();
    }
}
