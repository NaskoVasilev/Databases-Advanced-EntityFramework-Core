using System;
using System.IO;
using FastFood.Data;
using FastFood.Models;
using FastFood.Models.Enums;
using System.Linq;
using Newtonsoft.Json;
using FastFood.DataProcessor.Dto.Export;
using System.Xml.Serialization;
using System.Text;
using System.Xml;

namespace FastFood.DataProcessor
{
    public class Serializer
    {
        public static string ExportOrdersByEmployee(FastFoodDbContext context, string employeeName, string orderType)
        {
            OrderType type = Enum.Parse<OrderType>(orderType);

            var orders = context.Employees
                .Where(x => x.Name == employeeName)
                .ToArray()
                .Select(e => new
                {
                    Name = e.Name,
                    Orders = e.Orders
                    .Where(o => o.Type == type)
                    .Select(o => new
                    {
                        Customer = o.Customer,
                        Items = o.OrderItems.Select(i => new
                        {
                            Name = i.Item.Name,
                            Price = i.Item.Price,
                            Quantity = i.Quantity
                        })
                        .ToList(),
                        TotalPrice = o.TotalPrice
                    })
                    .OrderByDescending(o => o.TotalPrice)
                    .ThenByDescending(o => o.Items.Count)
                    .ToList(),
                    TotalMade = e.Orders.Where(o => o.Type == type).Sum(o => o.TotalPrice)
                })
                .FirstOrDefault();

            return JsonConvert.SerializeObject(orders);
        }

        public static string ExportCategoryStatistics(FastFoodDbContext context, string categoriesString)
        {
            var categoryNames = categoriesString.Split(",");

            var categories = context.Categories
                .Where(c => categoryNames.Contains(c.Name))
                .Select(c => new CategoryDto()
                {
                    Name = c.Name,
                    MostPopularItem = c.Items
                    .Select(i => new ItemDto()
                    {
                        Name = i.Name,
                        TotalMade = i.OrderItems.Sum(oi => oi.Item.Price * oi.Quantity),
                        TimesSold = i.OrderItems.Sum(oi => oi.Quantity)
                    })
                    .OrderByDescending(i => i.TotalMade)
                    .FirstOrDefault()
                })
                .OrderByDescending(c => c.MostPopularItem.TotalMade)
                .ThenByDescending(c => c.MostPopularItem.TimesSold)
                .ToArray();

            XmlSerializer serializer = new XmlSerializer(typeof(CategoryDto[]), new XmlRootAttribute("Categories"));
            StringBuilder sb = new StringBuilder();
            XmlSerializerNamespaces namespaces = new XmlSerializerNamespaces(new[] { XmlQualifiedName.Empty });

            serializer.Serialize(new StringWriter(sb), categories, namespaces);
            return sb.ToString().TrimEnd();
        }
    }
}