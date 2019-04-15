namespace SoftJail.DataProcessor
{

    using Data;
    using Newtonsoft.Json;
    using SoftJail.DataProcessor.ExportDto;
    using System;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Xml;
    using System.Xml.Serialization;

    public class Serializer
    {
        public static string ExportPrisonersByCells(SoftJailDbContext context, int[] ids)
        {
            var prisoners = context.Prisoners
                .Where(p => ids.Contains(p.Id))
                .Select(p => new
                {
                    Id = p.Id,
                    Name = p.FullName,
                    CellNumber = p.Cell.CellNumber,
                    Officers = p.PrisonerOfficers
                    .Select(po => new
                    {
                        OfficerName = po.Officer.FullName,
                        Department = po.Officer.Department.Name
                    })
                    .OrderBy(po => po.OfficerName)
                    .ToList(),
                    TotalOfficerSalary = p.PrisonerOfficers.Sum(po => po.Officer.Salary)
                })
                .OrderBy(p => p.Name)
                .ThenBy(p => p.Id)
                .ToList();

            return JsonConvert.SerializeObject(prisoners);
        }

        public static string ExportPrisonersInbox(SoftJailDbContext context, string prisonersNames)
        {
            string[] names = prisonersNames.Split(',');

            PrisonerDto[] prisoners = context.Prisoners
               .Where(p => names.Contains(p.FullName))
               .Select(p => new PrisonerDto()
               {
                   Id = p.Id,
                   Name = p.FullName,
                   IncarcerationDate = p.IncarcerationDate.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture),
                   EncryptedMessages = p.Mails.Select(m => new MailDto()
                   {
                       Description = ReverseString(m.Description)
                   })
                   .ToArray()
               })
                .OrderBy(p => p.Name)
                .ThenBy(p => p.Id)
                .ToArray();

            XmlSerializer serializer = new XmlSerializer(typeof(PrisonerDto[]), new XmlRootAttribute("Prisoners"));
            StringBuilder sb = new StringBuilder();
            StringWriter writer = new StringWriter(sb);

            XmlSerializerNamespaces namespaces = new XmlSerializerNamespaces(new[]
            {
                XmlQualifiedName.Empty
            });
            serializer.Serialize(writer, prisoners, namespaces);
            return sb.ToString().TrimEnd();
        }

        private static string ReverseString(string description)
        {
            return new string(description.ToCharArray().Reverse().ToArray());
        }
    }
}