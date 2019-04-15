namespace PetClinic.DataProcessor
{
    using PetClinic.Data;
    using System.Linq;
    using System.Globalization;
    using Newtonsoft.Json;
    using System.Xml.Serialization;
    using PetClinic.DataProcessor.Dtos.Export;
    using System.Text;
    using System.IO;
    using System.Xml;

    public class Serializer
    {
        public static string ExportAnimalsByOwnerPhoneNumber(PetClinicContext context, string phoneNumber)
        {
            var animals = context.Animals.
                Where(a => a.Passport.OwnerPhoneNumber == phoneNumber)
                .Select(a => new
                {
                    OwnerName = a.Passport.OwnerName,
                    AnimalName = a.Name,
                    Age = a.Age,
                    SerialNumber = a.PassportSerialNumber,
                    RegisteredOn = a.Passport.RegistrationDate.ToString("dd-MM-yyyy", CultureInfo.InvariantCulture)
                })
                .OrderBy(a => a.Age)
                .ThenBy(a => a.SerialNumber)
                .ToList();

            return JsonConvert.SerializeObject(animals);
        }

        public static string ExportAllProcedures(PetClinicContext context)
        {
            var procedures = context.Procedures
                .OrderBy(p => p.DateTime)
                .ThenBy(p => p.Animal.Passport.SerialNumber)
                .Select(p => new ProcedureDto()
                {
                    Passport = p.Animal.Passport.SerialNumber,
                    OwnerNumber = p.Animal.Passport.OwnerPhoneNumber,
                    DateTime = p.DateTime.ToString("dd-MM-yyyy", CultureInfo.InvariantCulture),
                    AnimalAids = p.ProcedureAnimalAids.Select(a => new AnimalAidDto()
                    {
                        Name = a.AnimalAid.Name,
                        Price = a.AnimalAid.Price
                    })
                    .ToList(),
                    TotalPrice = p.Cost
                })
                .ToArray();

            XmlSerializer serializer = new XmlSerializer(typeof(ProcedureDto[]), new XmlRootAttribute("Procedures"));
            StringBuilder sb = new StringBuilder();
            StringWriter writer = new StringWriter(sb);
            XmlSerializerNamespaces namespaces = new XmlSerializerNamespaces(new[]
            {
                XmlQualifiedName.Empty
            });

            serializer.Serialize(writer, procedures, namespaces);
            return sb.ToString().TrimEnd();
        }
    }
}
