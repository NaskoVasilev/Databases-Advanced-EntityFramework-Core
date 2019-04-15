namespace PetClinic.DataProcessor
{
    using System;

    using PetClinic.Data;
    using System.ComponentModel.DataAnnotations;
    using System.Collections.Generic;
    using Newtonsoft.Json;
    using PetClinic.Models;
    using System.Text;
    using System.Globalization;
    using System.Xml.Serialization;
    using System.IO;
    using PetClinic.DataProcessor.Dtos.Import;
    using AutoMapper;
    using System.Linq;

    public class Deserializer
    {

        public static string ImportAnimalAids(PetClinicContext context, string jsonString)
        {
            var animalAids = JsonConvert.DeserializeObject<AnimalAid[]>(jsonString);
            var animalAidNames = new HashSet<string>();
            var validAnimalAids = new List<AnimalAid>();
            var sb = new StringBuilder();

            foreach (var animalAid in animalAids)
            {
                if(!IsValid(animalAid) || animalAidNames.Contains(animalAid.Name))
                {
                    sb.AppendLine("Error: Invalid data.");
                    continue;
                }

                animalAidNames.Add(animalAid.Name);
                validAnimalAids.Add(animalAid);
                sb.AppendLine($"Record {animalAid.Name} successfully imported.");
            }

            context.AnimalAids.AddRange(validAnimalAids);
            context.SaveChanges();
            return sb.ToString().TrimEnd();
        }

        public static string ImportAnimals(PetClinicContext context, string jsonString)
        {
            JsonSerializerSettings settings = new JsonSerializerSettings
            {
                DateFormatString = "dd-MM-yyyy"
            };

            var animals = JsonConvert.DeserializeObject<Animal[]>(jsonString, settings);
            var serialNumbers = new HashSet<string>();
            var validAniamls = new List<Animal>();
            var sb = new StringBuilder();

            foreach (var animal in animals)
            {
                if(!IsValid(animal) || !IsValid(animal.Passport) || serialNumbers.Contains(animal.Passport.SerialNumber))
                {
                    sb.AppendLine("Error: Invalid data.");
                    continue;
                }

                serialNumbers.Add(animal.Passport.SerialNumber);
                validAniamls.Add(animal);
                sb.AppendLine($"Record {animal.Name} Passport №: {animal.Passport.SerialNumber} successfully imported.");
            }

            context.Animals.AddRange(validAniamls);
            context.SaveChanges();
            return sb.ToString().TrimEnd();
        }

        public static string ImportVets(PetClinicContext context, string xmlString)
        {
            var serializer = new XmlSerializer(typeof(ImportVetDto[]), new XmlRootAttribute("Vets"));
            var vets = (ImportVetDto[])serializer.Deserialize(new StringReader(xmlString));
            var phoneNumbers = new HashSet<string>();
            var sb = new StringBuilder();
            var validVets = new List<Vet>();

            foreach (var vet in vets)
            {
                if (!IsValid(vet) || phoneNumbers.Contains(vet.PhoneNumber))
                {
                    sb.AppendLine("Error: Invalid data.");
                    continue;
                }

                phoneNumbers.Add(vet.PhoneNumber);
                var validVet = Mapper.Map<Vet>(vet);
                validVets.Add(validVet);
                sb.AppendLine($"Record {vet.Name} successfully imported.");
            }

            context.Vets.AddRange(validVets);
            context.SaveChanges();
            return sb.ToString().TrimEnd();
        }

        public static string ImportProcedures(PetClinicContext context, string xmlString)
        {
            Dictionary<string, int> vetNames = context.Vets.ToDictionary(x => x.Name, y => y.Id);
            Dictionary<string, int> animals = context.Animals.ToDictionary(x => x.PassportSerialNumber, y => y.Id);
            Dictionary<string, int> animalAids = context.AnimalAids.ToDictionary(x => x.Name, y => y.Id);

            var serializer = new XmlSerializer(typeof(ImportProcedureDto[]), new XmlRootAttribute("Procedures"));
            var reader = new StringReader(xmlString);
            var procedures =  (ImportProcedureDto[])serializer.Deserialize(reader);
            var validProcedures = new List<Procedure>();
            var sb = new StringBuilder();

            foreach (var procedure in procedures)
            {
                int uniqueAnimalAidsCount = procedure.AnimalAids
                    .Select(a => a.Name)
                    .Distinct()
                    .ToArray().Length;


                if (!procedure.AnimalAids.All(IsValid) 
                    || !vetNames.ContainsKey(procedure.Vet) 
                    || !animals.ContainsKey(procedure.Animal)
                    || !procedure.AnimalAids.All(x => animalAids.ContainsKey(x.Name))
                    || uniqueAnimalAidsCount != procedure.AnimalAids.Count)
                {
                        sb.AppendLine("Error: Invalid data.");
                        continue;
                }

                var validProcedure = new Procedure()
                {
                    AnimalId = animals[procedure.Animal],
                    VetId = vetNames[procedure.Vet],
                    DateTime = DateTime.ParseExact(procedure.DateTime, "dd-MM-yyyy", CultureInfo.InvariantCulture),
                    ProcedureAnimalAids = procedure.AnimalAids.Select(a => new ProcedureAnimalAid()
                    {
                        AnimalAidId = animalAids[a.Name]
                    })
                    .ToList()
                };

                validProcedures.Add(validProcedure);
                sb.AppendLine($"Record successfully imported.");
            }

            context.Procedures.AddRange(validProcedures);
            context.SaveChanges();
            return sb.ToString().TrimEnd();
        }

        private static bool IsValid(object entity)
        {
            var context = new System.ComponentModel.DataAnnotations.ValidationContext(entity);
            var results = new List<ValidationResult>();

            bool isValid = Validator.TryValidateObject(entity, context, results, true);
            return isValid;
        }
    }
}
