namespace Cinema.DataProcessor
{
    using System;

    using Data;
    using System.Linq;
    using Newtonsoft.Json;
    using Cinema.DataProcessor.ExportDto;
    using System.Globalization;
    using System.Xml.Serialization;
    using System.Xml;
    using System.Text;
    using System.IO;

    public class Serializer
    {
        public static string ExportTopMovies(CinemaContext context, int rating)
        {
            var movies = context.Movies
                .Where(m => m.Rating >= rating && m.Projections.Any(p => p.Tickets.Any()))
                .OrderByDescending(m => m.Rating)
                .ThenByDescending(m => m.Projections.Sum(p => p.Tickets.Sum(t => t.Price)))
                .Select(m => new
                {
                    MovieName = m.Title,
                    Rating = m.Rating.ToString("F2"),
                    TotalIncomes = m.Projections.Sum(p => p.Tickets.Sum(t => t.Price)).ToString("F2"),
                    Customers = m.Projections.SelectMany(p => p.Tickets)
                    .Select(t => new
                    {
                        FirstName = t.Customer.FirstName,
                        LastName = t.Customer.LastName,
                        Balance = t.Customer.Balance.ToString("F2")
                    })
                    .OrderByDescending(c => c.Balance)
                    .ToList()
                })
                .Take(10)
                .ToList();

            return JsonConvert.SerializeObject(movies, Newtonsoft.Json.Formatting.Indented);
        }

        public static string ExportTopCustomers(CinemaContext context, int age)
        {
            var customers = context.Customers
                .Where(c => c.Age >= age)
                .OrderByDescending(c => c.Tickets.Sum(t => t.Price))
                .Select(c => new CustomerDto()
                {
                    FirstName = c.FirstName,
                    LastName = c.LastName,
                    SpentMoney = c.Tickets.Sum(t => t.Price).ToString("F2"),
                    SpentTime = new TimeSpan(c.Tickets.Sum(t => t.Projection.Movie.Duration.Ticks))
                    .ToString(@"hh\:mm\:ss", CultureInfo.InvariantCulture)
                })
                .Take(10)
                .ToArray();

            XmlSerializer serializer = new XmlSerializer(typeof(CustomerDto[]), new XmlRootAttribute("Customers"));
            XmlSerializerNamespaces namespaces = new XmlSerializerNamespaces(new[] { XmlQualifiedName.Empty });

            StringBuilder sb = new StringBuilder();
            serializer.Serialize(new StringWriter(sb), customers, namespaces);
            return sb.ToString().TrimEnd();
        }
    }
}