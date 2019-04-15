using AutoMapper;
using CarDealer.Data;
using CarDealer.Dtos.Export;
using CarDealer.Dtos.Import;
using CarDealer.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace CarDealer
{
    public class StartUp
    {
        private const string ImportMessage = "Successfully imported {0}";

        public static void Main(string[] args)
        {
            Mapper.Initialize(x => x.AddProfile<CarDealerProfile>());

            using (var context = new CarDealerContext())
            {
                string result = GetSalesWithAppliedDiscount(context);
                Console.WriteLine(result);
            }
        }

        public static string ImportSuppliers(CarDealerContext context, string inputXml)
        {
            var suppliers = DeserializeXml<ImportSuppliersDto[]>(inputXml, "Suppliers")
                .Select(s => Mapper.Map<Supplier>(s))
                .ToList();

            context.Suppliers.AddRange(suppliers);
            context.SaveChanges();
            return string.Format(ImportMessage, suppliers.Count);
        }

        public static string ImportParts(CarDealerContext context, string inputXml)
        {
            HashSet<int> suppllierIds = context.Suppliers.Select(s => s.Id).ToHashSet();

            var parts = DeserializeXml<ImportPartDto[]>(inputXml, "Parts")
                .Where(x => suppllierIds.Contains(x.SupplierId))
                .Select(x => Mapper.Map<Part>(x))
                .ToList();

            context.Parts.AddRange(parts);
            context.SaveChanges();
            return string.Format(ImportMessage, parts.Count);
        }

        public static string ImportCars(CarDealerContext context, string inputXml)
        {
            HashSet<int> partIds = context.Parts.Select(s => s.Id).ToHashSet();

            var cars = DeserializeXml<CarImportDto[]>(inputXml, "Cars")
                .Where(x => x.Parts.All(p => partIds.Contains(p.Id)))
                .Select(x => new Car()
                {
                    Make = x.Make,
                    Model = x.Model,
                    TravelledDistance = x.TraveledDistance,
                    PartCars = x.Parts.Select(p => new PartCar() { PartId = p.Id }).ToHashSet()
                })
                .ToList();

            context.Cars.AddRange(cars);
            context.SaveChanges();
            return string.Format(ImportMessage, cars.Count);
        }

        public static string ImportCustomers(CarDealerContext context, string inputXml)
        {
            var customers = DeserializeXml<ImportCustomerDto[]>(inputXml, "Customers")
                .Select(x => Mapper.Map<Customer>(x))
                .ToList();

            context.Customers.AddRange(customers);
            context.SaveChanges();
            return string.Format(ImportMessage, customers.Count);
        }

        public static string ImportSales(CarDealerContext context, string inputXml)
        {
            HashSet<int> carIds = context.Cars.Select(c => c.Id).ToHashSet();

            var sales = DeserializeXml<ImportSaleDto[]>(inputXml, "Sales")
                .Where(c => carIds.Contains(c.CarId))
                .Select(x => Mapper.Map<Sale>(x))
                .ToList();

            context.Sales.AddRange(sales);
            context.SaveChanges();
            return string.Format(ImportMessage, sales.Count);
        }

        public static string GetCarsWithDistance(CarDealerContext context)
        {
            var cars = context.Cars
                .Where(c => c.TravelledDistance > 2000000)
                .OrderBy(c => c.Make)
                .ThenBy(c => c.Model)
                .Take(10)
                .Select(c => Mapper.Map<CarDto>(c))
                .ToList();

            return SerializeToXml(cars, "cars");
        }

        public static string GetCarsFromMakeBmw(CarDealerContext context)
        {
            var cars = context.Cars
                .Where(c => c.Make == "BMW")
                .OrderBy(c => c.Model)
                .ThenByDescending(c => c.TravelledDistance)
                .Select(c => Mapper.Map<CarInfoDto>(c))
                .ToList();

            return SerializeToXml(cars, "cars");
        }

        public static string GetLocalSuppliers(CarDealerContext context)
        {
            var suppliers = context.Suppliers
                .Where(s => !s.IsImporter)
                .Select(s => Mapper.Map<NativeSupplierDto>(s))
                .ToList();

            return SerializeToXml(suppliers, "suppliers");
        }

        public static string GetCarsWithTheirListOfParts(CarDealerContext context)
        {
            var cars = context.Cars
                .OrderByDescending(c => c.TravelledDistance)
                .ThenBy(c => c.Model)
                .Take(5)
                .Select(c => new CarWithPartsDto()
                {
                    Make = c.Make,
                    Model = c.Model,
                    TravelledDistance = c.TravelledDistance,
                    Parts = c.PartCars.Select(cp => new PartDto()
                    {
                        Name = cp.Part.Name,
                        Price = cp.Part.Price
                    })
                    .OrderByDescending(p => p.Price)
                    .ToList()
                })
                .ToList();

            return SerializeToXml(cars, "cars");
        }

        public static string GetTotalSalesByCustomer(CarDealerContext context)
        {
            var customers = context.Customers
                .Where(c => c.Sales.Any())
                .Select(c => new CustomerDto()
                {
                    FullName = c.Name,
                    BoughtCars = c.Sales.Count,
                    SpentMoney = c.Sales.Sum(s => s.Car.PartCars.Sum(p => p.Part.Price))
                })
                .OrderByDescending(c => c.SpentMoney)
                .ToList();

            return SerializeToXml(customers, "customers");
        }

        public static T DeserializeXml<T>(string inputXml, string rootName)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(T), new XmlRootAttribute(rootName));
            T result = default(T);

            using (var reader = new StringReader(inputXml))
            {
                result = (T)serializer.Deserialize(reader);
            }

            return result;
        }

        public static string GetSalesWithAppliedDiscount(CarDealerContext context)
        {
            var sales = context.Sales
                .Select(s => new SaleDto()
                {
                    Car = Mapper.Map<CarSaleDto>(s.Car),
                    CustomerName = s.Customer.Name,
                    Discount = (int)s.Discount,
                    Price = s.Car.PartCars.Sum(p => p.Part.Price),
                    PriceWithDiscount = ((s.Car.PartCars.Sum(p => p.Part.Price)) * (1 - (s.Discount / 100M))).ToString("G29")
                })
                .ToList();
            return SerializeToXml(sales, "sales");
        }

        public static string SerializeToXml<T>(T obj, string rootName)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(T), new XmlRootAttribute(rootName));

            XmlSerializerNamespaces namespaces = new XmlSerializerNamespaces(new XmlQualifiedName[]
            {
                XmlQualifiedName.Empty
            });

            StringBuilder sb = new StringBuilder();
            StringWriter writer = new StringWriter(sb);
            serializer.Serialize(writer, obj, namespaces);

            return sb.ToString().TrimEnd();
        }
    }
}