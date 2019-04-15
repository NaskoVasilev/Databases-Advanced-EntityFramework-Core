using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using AutoMapper;
using CarDealer.Data;
using CarDealer.Models;
using Newtonsoft.Json;
using CarDealer.DTO.Import;
using CarDealer.DTO.Export;
using Newtonsoft.Json.Serialization;

namespace CarDealer
{
    public class StartUp
    {
        private const string ImportMessage = "Successfully imported {0}.";

        public static void Main(string[] args)
        {
            //string path = "./Datasets/sales.json";
            //string json = File.ReadAllText(path);

            Mapper.Initialize(configuration => configuration.AddProfile<CarDealerProfile>());

            using (var context = new CarDealerContext())
            {
                string result = GetSalesWithAppliedDiscount(context);
                Console.WriteLine(result);
            }
        }

        public static string ImportSuppliers(CarDealerContext context, string inputJson)
        {
            List<Supplier> suppliers = JsonConvert.DeserializeObject<List<Supplier>>(inputJson);

            context.Suppliers.AddRange(suppliers);
            context.SaveChanges();
            return string.Format(ImportMessage, suppliers.Count);
        }

        public static string ImportParts(CarDealerContext context, string inputJson)
        {
            HashSet<int> supplierIds = context.Suppliers.Select(s => s.Id).ToHashSet();

            List<Part> parts = JsonConvert.DeserializeObject<List<Part>>(inputJson)
                .Where(p => supplierIds.Contains(p.SupplierId))
                .ToList();

            context.Parts.AddRange(parts);
            context.SaveChanges();
            return string.Format(ImportMessage, parts.Count);
        }

        public static string ImportCars(CarDealerContext context, string inputJson)
        {
            List<Car> cars = JsonConvert.DeserializeObject<List<CarImport>>(inputJson)
                .Select(c => Mapper.Map<Car>(c))
                .ToList();

            context.Cars.AddRange(cars);
            context.SaveChanges();
            return string.Format(ImportMessage, cars.Count);
        }

        public static string ImportCustomers(CarDealerContext context, string inputJson)
        {
            List<Customer> customers = JsonConvert.DeserializeObject<List<Customer>>(inputJson);

            context.Customers.AddRange(customers);
            context.SaveChanges();
            return string.Format(ImportMessage, customers.Count);
        }

        public static string ImportSales(CarDealerContext context, string inputJson)
        {
            List<Sale> sales = JsonConvert.DeserializeObject<List<Sale>>(inputJson);

            context.Sales.AddRange(sales);
            context.SaveChanges();
            return string.Format(ImportMessage, sales.Count);
        }

        public static string GetOrderedCustomers(CarDealerContext context)
        {
            var result = context.Customers
                .OrderBy(c => c.BirthDate)
                .ThenBy(c => c.IsYoungDriver)
                .Select(c => Mapper.Map<CustomerDto>(c))
                .ToList();

            return JsonConvert.SerializeObject(result, new JsonSerializerSettings()
            {
                DateFormatString = "dd/MM/yyyy"
            });
        }

        public static string GetCarsFromMakeToyota(CarDealerContext context)
        {
            var cars = context.Cars
                .Where(c => c.Make == "Toyota")
                .OrderBy(c => c.Model)
                .ThenByDescending(c => c.TravelledDistance)
                .Select(c => Mapper.Map<CarDto>(c))
                .ToList();

            return JsonConvert.SerializeObject(cars);
        }

        public static string GetLocalSuppliers(CarDealerContext context)
        {
            var suppliers = context.Suppliers
                .Where(s => !s.IsImporter)
                .Select(s => Mapper.Map<LocalSuppliersDto>(s))
                .ToList();

            return JsonConvert.SerializeObject(suppliers);
        }

        public static string GetCarsWithTheirListOfParts(CarDealerContext context)
        {
            var cars = context.Cars
                .Select(c => new CarPartDto()
                {
                    Car = Mapper.Map<CarInfoDto>(c),
                    Parts = c.PartCars
                        .Select(p => new PartDto()
                        {
                            Name = p.Part.Name,
                            Price = $"{p.Part.Price:F2}"
                        })
                        .ToList()
                })
                .ToList();

            return JsonConvert.SerializeObject(cars);
        }

        public static string GetTotalSalesByCustomer(CarDealerContext context)
        {
            var customers = context.Customers
                .Where(c => c.Sales.Count >= 1)
                .Select(c => new
                {
                    FullName = c.Name,
                    BoughtCars = c.Sales.Count,
                    SpentMoney = c.Sales
                    .Sum(s => s.Car.PartCars.Sum(cp => cp.Part.Price))
                })
                .OrderByDescending(c => c.SpentMoney)
                .ThenByDescending(c => c.BoughtCars)
                .ToList();

            DefaultContractResolver contractResolver = new DefaultContractResolver()
            {
                NamingStrategy = new CamelCaseNamingStrategy()
            };

            string result = JsonConvert.SerializeObject(customers, new JsonSerializerSettings()
            {
                ContractResolver = contractResolver,
            });
            return result;
        }

        public static string GetSalesWithAppliedDiscount(CarDealerContext context)
        {
            var sales = context.Sales
                .Select(s => new SaleDto
                {
                    Car = Mapper.Map<CarInfoDto>(s.Car),
                    CustomerName = s.Customer.Name,
                    Discount = $"{s.Discount:F2}",
                    Price = $"{s.Car.PartCars.Sum(cp => cp.Part.Price):F2}",
                    PriceWithDiscount = $"{s.Car.PartCars.Sum(cp => cp.Part.Price) * (1 - (s.Discount / 100)):F2}"
                })
                .Take(10)
                .ToList();

            return JsonConvert.SerializeObject(sales);
        }
    }
}