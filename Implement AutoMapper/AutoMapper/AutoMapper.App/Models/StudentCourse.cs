using System;

namespace AutoMapper.App.Models
{
    public class StudentCourse
    {
        public string Name { get; set; }

        public CourseStatistic Statistic { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }
    }
}
