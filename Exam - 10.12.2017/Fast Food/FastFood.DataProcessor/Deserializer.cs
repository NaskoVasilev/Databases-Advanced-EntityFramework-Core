using System;
using FastFood.Data;
using Newtonsoft.Json;
using FastFood.DataProcessor.Dto.Import;
using System.Collections.Generic;
using FastFood.Models;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Xml.Serialization;
using System.IO;
using System.Globalization;
using FastFood.Models.Enums;

namespace FastFood.DataProcessor
{
    public static class Deserializer
    {
        private const string FailureMessage = "Invalid data format.";
        private const string SuccessMessage = "Record {0} successfully imported.";

        public static string ImportEmployees(FastFoodDbContext context, string jsonString)
        {
            var employees = JsonConvert.DeserializeObject<ImportEmployeeDto[]>(jsonString);
            var positions = new Dictionary<string, Position>();
            var validEmployees = new List<Employee>();
            var sb = new StringBuilder();

            foreach (var employee in employees)
            {
                if (!IsValid(employee))
                {
                    sb.AppendLine("Invalid data format.");
                    continue;
                }

                positions.TryGetValue(employee.Position, out Position validPosition);

                if (validPosition == null)
                {
                    validPosition = new Position() { Name = employee.Position };

                    if (!IsValid(validPosition))
                    {
                        sb.AppendLine("Invalid data format.");
                        continue;
                    }
                    positions.Add(employee.Position, validPosition);
                }

                Employee validEmployee = new Employee
                {
                    Name = employee.Name,
                    Age = employee.Age,
                    Position = validPosition
                };

                validEmployees.Add(validEmployee);
                sb.AppendLine($"Record {employee.Name} successfully imported.");
            }

            context.Employees.AddRange(validEmployees);
            context.SaveChanges();
            return sb.ToString().TrimEnd();
        }

        public static string ImportItems(FastFoodDbContext context, string jsonString)
        {
            var items = JsonConvert.DeserializeObject<ImportItemDto[]>(jsonString);
            var categories = new Dictionary<string, Category>();
            var validItems = new Dictionary<string, Item>();
            var sb = new StringBuilder();

            foreach (var item in items)
            {
                if (!IsValid(item) || validItems.ContainsKey(item.Name))
                {
                    sb.AppendLine("Invalid data format.");
                    continue;
                }

                categories.TryGetValue(item.Category, out Category validCategory);

                if (validCategory == null)
                {
                    validCategory = new Category() { Name = item.Category };

                    if (!IsValid(validCategory))
                    {
                        sb.AppendLine("Invalid data format.");
                        continue;
                    }
                    categories.Add(item.Category, validCategory);
                }

                Item validItem = new Item
                {
                    Name = item.Name,
                    Price = item.Price,
                    Category = validCategory
                };

                validItems.Add(validItem.Name, validItem);
                sb.AppendLine($"Record {item.Name} successfully imported.");
            }

            context.Items.AddRange(validItems.Values);
            context.SaveChanges();
            return sb.ToString().TrimEnd();
        }

        public static string ImportOrders(FastFoodDbContext context, string xmlString)
        {
            Dictionary<string, int> employees = context.Employees.ToDictionary(x => x.Name, y => y.Id);
            Dictionary<string, int> items = context.Items.ToDictionary(x => x.Name, y => y.Id);

            XmlSerializer serializer = new XmlSerializer(typeof(ImportOrderDto[]), new XmlRootAttribute("Orders"));
            var orders = (ImportOrderDto[])serializer.Deserialize(new StringReader(xmlString));
            List<Order> validOrders = new List<Order>();
            StringBuilder sb = new StringBuilder();

            foreach (var order in orders)
            {
                if (!IsValid(order)
                   || !employees.ContainsKey(order.Employee)
                   || !order.Items.All(x => items.ContainsKey(x.Name)))
                {
                    sb.AppendLine("Invalid data format.");
                    continue;
                }

                bool isValid = Enum.TryParse<OrderType>(order.Type, out OrderType type);
                if(!isValid)
                {
                    sb.AppendLine("Invalid data format.");
                    continue;
                }

                Order validOrder = new Order()
                {
                    Customer = order.Customer,
                    DateTime = DateTime.ParseExact(order.DateTime, "dd/MM/yyyy HH:mm", CultureInfo.InvariantCulture),
                    Type = type,
                    EmployeeId = employees[order.Employee],
                    OrderItems = order.Items.Select(i => new OrderItem()
                    {
                        ItemId = items[i.Name],
                        Quantity = i.Quantity
                    })
                    .ToList()
                };

                validOrders.Add(validOrder);
                sb.AppendLine($"Order for {order.Customer} on {order.DateTime} added");
            }

            context.Orders.AddRange(validOrders);
            context.SaveChanges();
            return sb.ToString().TrimEnd();
        }

        private static bool IsValid(object entity)
        {
            var context = new ValidationContext(entity);
            var results = new List<ValidationResult>();

            return Validator.TryValidateObject(entity, context, results, true);
        }
    }
}