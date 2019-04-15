using AutoMapper.App.Models;
using Newtonsoft.Json;
using System;
using AutoMapper.App.ViewModels;
using System.Collections.Generic;

namespace AutoMapper.App
{
    class StartUp
    {
        static void Main(string[] args)
        {
            Student student = new Student()
            {
                Name = "Atanas Vasilev",
                Age = 17,
                NumberInClass = 2,
                Class = 11
            };

            string[] courseNames = new string[] { "C# Fund", "C# DB", "C# Web" };
            Random random = new Random();
            for (int i = 0; i < 3; i++)
            {
                student.StudentCourses.Add(new StudentCourse()
                {
                    Name = courseNames[i],
                    Statistic = new CourseStatistic()
                    {
                        Grade = new List<double>() { random.Next(2, 6), random.Next(2, 6), random.Next(2, 6) },
                        Hours = random.Next(25, 30),
                        Lections = random.Next(5, 8)
                    },
                    StartDate = DateTime.Now
                });
            }

            MapperConfiguration configuration = new MapperConfiguration();
            configuration.CreateMap();

            var person = configuration.Mapper.CreateMappedObject<StudentViewModel>(student);

            Console.WriteLine(JsonConvert.SerializeObject(person));
        }
    }
}
