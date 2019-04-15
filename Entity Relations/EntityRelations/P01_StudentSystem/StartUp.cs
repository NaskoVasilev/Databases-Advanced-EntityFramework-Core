using System;
using P01_StudentSystem.Data;
using P01_StudentSystem.Data.Models;
using System.Linq;
using System.Collections.Generic;
using P01_StudentSystem.Enums;

namespace P01_StudentSystem
{
    public class StartUp
    {
        public static void Main(string[] args)
        {
            using (StudentSystemContext context = new StudentSystemContext())
            {
                context.Database.EnsureDeleted();
                context.Database.EnsureCreated();

                SeedStudents(context);
                SeedCourses(context);
                SeedResources(context);
                SeedHomeworkSubmissions(context);
                SeedStudentCoureses(context);
            }
        }

        private static void SeedStudentCoureses(StudentSystemContext context)
        {
            List<Course> Courses = context.Courses.ToList();
            List<Student> Students = context.Students.ToList();

            for (int i = 1; i <= 20; i++)
            {
                string content = "Content" + i;

                context.StudentCourses.Add(new StudentCourse()
                {
                    Course = Courses[i % Courses.Count],
                    Student = Students[i % Students.Count]
                });
            }

            context.SaveChanges();
        }

        private static void SeedHomeworkSubmissions(StudentSystemContext context)
        {
            List<Course> Courses = context.Courses.ToList();
            List<Student> Students = context.Students.ToList();

            for (int i = 1; i <= 20; i++)
            {
                string content = "Content" + i;

                context.HomeworkSubmissions.Add(new Homework()
                {
                    Content = content,
                    SubmissionTime = DateTime.Now,
                    ContentType = (ContentType)(i % 3),
                    Course = Courses[i % Courses.Count],
                    Student = Students[i % Students.Count]
                });
            }

            context.SaveChanges();
        }

        private static void SeedResources(StudentSystemContext context)
        {
            List<Course> Courses = context.Courses.ToList();

            for (int i = 1; i <= 20; i++)
            {
                string name = "Resource" + i;
                string url = "https://resource/" + i;

                context.Resources.Add(new Resource()
                {
                    Name = name,
                    Url = url,
                    ResourceType = (ResourceType)(i % 4),
                    Course = Courses[i % Courses.Count]
                });
            }

            context.SaveChanges();
        }

        private static void SeedCourses(StudentSystemContext context)
        {
            for (int i = 1; i <= 20; i++)
            {
                string name = "Student" + i;
                string description = "Description" + i;

                context.Courses.Add(new Course()
                {
                    Name = name,
                    Description = description,
                    EndDate = new DateTime(2019, 3, i),
                    Price = i * 10,
                    StartDate = new DateTime(2019, 1, i)
                });
            }

            context.SaveChanges();
        }

        private static void SeedStudents(StudentSystemContext context)
        {
            for (int i = 1; i <= 20; i++)
            {
                string name = "Student" + i;
                string phoneNumber = new string(i.ToString()[0], 10);

                context.Students.Add(new Student()
                {
                    Name = name,
                    PhoneNumber = phoneNumber,
                    RegisteredOn = DateTime.Now,
                    Birthday = new DateTime(1995 + i % 10, i % 12 + 1, i)
                });
            }

            context.SaveChanges();
        }
    }
}
