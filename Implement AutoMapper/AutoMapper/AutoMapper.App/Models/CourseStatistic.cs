using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AutoMapper.App.Models
{
    public class CourseStatistic
    {
        public List<double> Grade { get; set; }

        public int Hours { get; set; }

        public int Lections { get; set; }

        public double AverageGrade => this.Grade.Average();
    }
}
