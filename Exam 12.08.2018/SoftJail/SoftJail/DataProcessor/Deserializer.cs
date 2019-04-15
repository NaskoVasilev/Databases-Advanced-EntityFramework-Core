namespace SoftJail.DataProcessor
{
    using AutoMapper;
    using Data;
    using Newtonsoft.Json;
    using SoftJail.Data.Models;
    using SoftJail.Data.Models.Enums;
    using SoftJail.DataProcessor.ImportDto;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Xml.Serialization;

    public class Deserializer
    {
        public static string ImportDepartmentsCells(SoftJailDbContext context, string jsonString)
        {
            List<ImportDepartmentDto> departments = JsonConvert.DeserializeObject<List<ImportDepartmentDto>>(jsonString);
            List<Department> validDepartments = new List<Department>();
            StringBuilder sb = new StringBuilder();

            foreach (var departmentDto in departments)
            {
                if(!IsValid(departmentDto) || !departmentDto.Cells.All(IsValid))
                {
                    sb.AppendLine("Invalid Data");
                    continue;
                }

                Department department = new Department()
                {
                    Name = departmentDto.Name,
                    Cells = departmentDto.Cells.Select(c => Mapper.Map<Cell>(c)).ToList()
                };

                validDepartments.Add(department);
                sb.AppendLine($"Imported {department.Name} with {department.Cells.Count} cells");
            }

            context.Departments.AddRange(validDepartments);
            context.SaveChanges();
            return sb.ToString().TrimEnd();
        }

        public static string ImportPrisonersMails(SoftJailDbContext context, string jsonString)
        {
            List<ImportPrisonerDto> prisoners = JsonConvert.DeserializeObject<List<ImportPrisonerDto>>(jsonString);
            List<Prisoner> validPrisoners = new List<Prisoner>();
            StringBuilder sb = new StringBuilder();

            foreach (var prisoner in prisoners)
            {
                if(!IsValid(prisoner) || !prisoner.Mails.All(IsValid))
                {
                    sb.AppendLine("Invalid Data");
                    continue;
                }

                DateTime? releaseDate = null;
                if(prisoner.ReleaseDate != null)
                {
                    releaseDate = DateTime.ParseExact(prisoner.ReleaseDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                }

                Prisoner validPrisoner = new Prisoner()
                {
                    FullName = prisoner.FullName,
                    Nickname = prisoner.Nickname,
                    Age = prisoner.Age,
                    IncarcerationDate = DateTime.ParseExact(prisoner.IncarcerationDate, "dd/MM/yyyy", CultureInfo.InvariantCulture),
                    ReleaseDate = releaseDate,
                    Bail = prisoner.Bail,
                    CellId = prisoner.CellId,
                    Mails = prisoner.Mails.Select(m => Mapper.Map<Mail>(m)).ToList()
                };

                validPrisoners.Add(validPrisoner);
                sb.AppendLine($"Imported {prisoner.FullName} {prisoner.Age} years old");
            }

            context.Prisoners.AddRange(validPrisoners);
            context.SaveChanges();
            return sb.ToString().TrimEnd();
        }

        public static string ImportOfficersPrisoners(SoftJailDbContext context, string xmlString)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(ImportOfficerDto[]), new XmlRootAttribute("Officers"));
            StringReader reader = new StringReader(xmlString);
            ImportOfficerDto[] officers = (ImportOfficerDto[])serializer.Deserialize(reader);

            StringBuilder sb = new StringBuilder();
            List<Officer> validOfficers = new List<Officer>();

            foreach (var officer in officers)
            {
                if (!IsValid(officer))
                {
                    sb.AppendLine("Invalid Data");
                    continue;
                }


                bool isValidPosition = Enum.TryParse<Position>(officer.Position, out Position position);
                bool isValidWeapon = Enum.TryParse<Weapon>(officer.Weapon, out Weapon weapon);

                if(!isValidPosition || !isValidWeapon)
                {
                    sb.AppendLine("Invalid Data");
                    continue;
                }

                Officer validOfficer = new Officer()
                {
                    FullName = officer.Name,
                    Salary = officer.Money,
                    Position = position,
                    Weapon = weapon,
                    DepartmentId = officer.DepartmentId,
                    OfficerPrisoners = officer.Prisoners.Select(p => new OfficerPrisoner() { PrisonerId = p.Id }).ToList()
                };

                validOfficers.Add(validOfficer);
                sb.AppendLine($"Imported {officer.Name} ({officer.Prisoners.Count} prisoners)");
            }

            context.Officers.AddRange(validOfficers);
            context.SaveChanges();
            return sb.ToString().TrimEnd();
        }

        private static bool IsValid(object entity)
        {
            System.ComponentModel.DataAnnotations.ValidationContext context = 
                new System.ComponentModel.DataAnnotations.ValidationContext(entity);
            List<ValidationResult> validationResults = new List<ValidationResult>();

            bool isValid = Validator.TryValidateObject(entity, context, validationResults, true);
            return isValid;
        }
    }
}