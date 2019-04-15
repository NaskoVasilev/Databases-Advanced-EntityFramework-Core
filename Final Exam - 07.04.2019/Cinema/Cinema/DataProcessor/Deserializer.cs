namespace Cinema.DataProcessor
{
    using System;

    using Data;
    using System.Collections.Generic;
    using Cinema.Data.Models;
    using System.Text;
    using Newtonsoft.Json;
    using System.ComponentModel.DataAnnotations;
    using Cinema.DataProcessor.ImportDto;
    using System.Xml.Serialization;
    using System.IO;
    using System.Linq;
    using System.Globalization;

    public class Deserializer
    {
        private const string ErrorMessage = "Invalid data!";
        private const string SuccessfulImportMovie 
            = "Successfully imported {0} with genre {1} and rating {2}!";
        private const string SuccessfulImportHallSeat 
            = "Successfully imported {0}({1}) with {2} seats!";
        private const string SuccessfulImportProjection 
            = "Successfully imported projection {0} on {1}!";
        private const string SuccessfulImportCustomerTicket 
            = "Successfully imported customer {0} {1} with bought tickets: {2}!";

        public static string ImportMovies(CinemaContext context, string jsonString)
        {
            var movies = JsonConvert.DeserializeObject<Movie[]>(jsonString);

            HashSet<string> titles = new HashSet<string>();
            List<Movie> validMovies = new List<Movie>();
            StringBuilder sb = new StringBuilder();

            foreach (var movie in movies)
            {
                if (!IsValid(movie) || titles.Contains(movie.Title))
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                titles.Add(movie.Title);
                validMovies.Add(movie);
                sb.AppendLine(string.Format(SuccessfulImportMovie, movie.Title, movie.Genre, movie.Rating.ToString("F2")));
            }

            context.Movies.AddRange(validMovies);
            context.SaveChanges();
            return sb.ToString().TrimEnd();
        }

        public static string ImportHallSeats(CinemaContext context, string jsonString)
        {
            var halls = JsonConvert.DeserializeObject<HallImportDto[]>(jsonString);
            List<Hall> validHalls = new List<Hall>();
            StringBuilder sb = new StringBuilder();

            foreach (var hall in halls)
            {
                if (!IsValid(hall))
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                List<Seat> seats = GenerateSeats(hall.Seats);

                Hall validHall = new Hall()
                {
                    Name = hall.Name,
                    Is4Dx = hall.Is4Dx,
                    Is3D = hall.Is3D,
                    Seats = seats
                };

                validHalls.Add(validHall);
                string projectionType = GetProjectionType(hall);
                sb.AppendLine(string.Format(SuccessfulImportHallSeat, hall.Name, projectionType, hall.Seats));
            }

            context.Halls.AddRange(validHalls);
            context.SaveChanges();
            return sb.ToString().TrimEnd();
        }

        public static string ImportProjections(CinemaContext context, string xmlString)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(ProjectionImportDto[]), new XmlRootAttribute("Projections"));
            ProjectionImportDto[] projections = (ProjectionImportDto[])serializer.Deserialize(new StringReader(xmlString));

            StringBuilder sb = new StringBuilder();
            List<Projection> validProjections = new List<Projection>();
            HashSet<int> hallIds = context.Halls.Select(m => m.Id).ToHashSet();

            foreach (var projection in projections)
            {
                Movie movie = context.Movies.FirstOrDefault(m => m.Id == projection.MovieId);

                if (!IsValid(projection) || movie == null || !hallIds.Contains(projection.HallId))
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                Projection validProjection = new Projection()
                {
                    HallId = projection.HallId,
                    MovieId = projection.MovieId,
                    DateTime = DateTime.ParseExact(projection.DateTime, "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture)
                };
                validProjections.Add(validProjection);
                sb.AppendLine(string.Format(SuccessfulImportProjection, movie.Title, 
                    validProjection.DateTime.ToString("MM/dd/yyyy", CultureInfo.InvariantCulture)));
            }

            context.Projections.AddRange(validProjections);
            context.SaveChanges();
            return sb.ToString().TrimEnd();
        }

        public static string ImportCustomerTickets(CinemaContext context, string xmlString)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(CustomerImportDto[]), new XmlRootAttribute("Customers"));
            CustomerImportDto[] customers = (CustomerImportDto[])serializer.Deserialize(new StringReader(xmlString));

            StringBuilder sb = new StringBuilder();
            List<Customer> validCustomers = new List<Customer>();
            HashSet<int> projectionIds = context.Projections.Select(m => m.Id).ToHashSet();

            foreach (var customer in customers)
            {
                if (!IsValid(customer))
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                bool ticketsAreValid = customer.Tickets.All(t => IsValid(t) && projectionIds.Contains(t.ProjectionId));
                if (!ticketsAreValid)
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                Customer validCustomer = new Customer()
                {
                    FirstName = customer.FirstName,
                    LastName = customer.LastName,
                    Age = customer.Age,
                    Balance = customer.Balance,
                    Tickets = customer.Tickets.Select(t => new Ticket()
                    {
                        Price = t.Price,
                        ProjectionId = t.ProjectionId
                    })
                    .ToList()
                };

                validCustomers.Add(validCustomer);
                string message = string.Format(SuccessfulImportCustomerTicket, customer.FirstName, customer.LastName, customer.Tickets.Length);
                sb.AppendLine(message);
            }

            context.Customers.AddRange(validCustomers);
            context.SaveChanges();
            return sb.ToString().TrimEnd();
        }

        private static bool IsValid(object entity)
        {
            var context = new ValidationContext(entity);
            var results = new List<ValidationResult>();

            bool isValid = Validator.TryValidateObject(entity, context, results, true);
            return isValid;
        }

        private static string GetProjectionType(HallImportDto hall)
        {
            string type = "";
            if (hall.Is4Dx)
            {
                type = "4Dx";
                if (hall.Is3D)
                {
                    type += "/3D";
                }
            }
            else if (hall.Is3D)
            {
                type = "3D";
            }
            else
            {
                type = "Normal";
            }

            return type;
        }

        private static List<Seat> GenerateSeats(int seatsCount)
        {
            List<Seat> seats = new List<Seat>(seatsCount);

            for (int i = 0; i < seatsCount; i++)
            {
                seats.Add(new Seat() { });
            }

            return seats;
        }
    }
}