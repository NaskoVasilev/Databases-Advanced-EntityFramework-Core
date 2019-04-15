using AutoMapper;
using ProductShop.Data;
using ProductShop.Dtos.Export;
using ProductShop.Dtos.Import;
using ProductShop.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace ProductShop
{
    public class StartUp
    {
        private const string ImportMessage = "Successfully imported {0}";

        public static void Main(string[] args)
        {
            Mapper.Initialize(x => x.AddProfile<ProductShopProfile>());

            using (var context = new ProductShopContext())
            {
                string result = GetUsersWithProducts(context);
                Console.WriteLine(result);
            }
        }

        public static string ImportUsers(ProductShopContext context, string inputXml)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(ImportUserDto[]), new XmlRootAttribute("Users"));

            User[] users = ((ImportUserDto[])serializer.Deserialize(new StringReader(inputXml)))
                .Select(u => Mapper.Map<User>(u))
                .ToArray();

            context.Users.AddRange(users);
            context.SaveChanges();
            return string.Format(ImportMessage, users.Length);
        }

        public static string ImportProducts(ProductShopContext context, string inputXml)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(ImportProductDto[]), new XmlRootAttribute("Products"));

            Product[] products = ((ImportProductDto[])serializer.Deserialize(new StringReader(inputXml)))
                .Select(u => Mapper.Map<Product>(u))
                .ToArray();

            context.Products.AddRange(products);
            context.SaveChanges();
            return string.Format(ImportMessage, products.Length);
        }

        public static string ImportCategories(ProductShopContext context, string inputXml)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(ImportCategoryDto[]), new XmlRootAttribute("Categories"));

            Category[] categories = ((ImportCategoryDto[])serializer.Deserialize(new StringReader(inputXml)))
                .Where(u => u.Name != null)
                .Select(u => Mapper.Map<Category>(u))
                .ToArray();

            context.Categories.AddRange(categories);
            context.SaveChanges();
            return string.Format(ImportMessage, categories.Length);
        }

        public static string ImportCategoryProducts(ProductShopContext context, string inputXml)
        {
            HashSet<int> productIds = context.Products.Select(p => p.Id).ToHashSet();
            HashSet<int> categoryIds = context.Categories.Select(c => c.Id).ToHashSet();

            XmlSerializer serializer = new XmlSerializer(typeof(ImportCategoryProductDto[]), new XmlRootAttribute("CategoryProducts"));

            CategoryProduct[] categoryPrdocuts = ((ImportCategoryProductDto[])serializer.Deserialize(new StringReader(inputXml)))
                .Where(cp => productIds.Contains(cp.ProductId) && categoryIds.Contains(cp.CategoryId))
                .Select(cp => Mapper.Map<CategoryProduct>(cp))
                .ToArray();

            context.CategoryProducts.AddRange(categoryPrdocuts);
            context.SaveChanges();
            return string.Format(ImportMessage, categoryPrdocuts.Length);
        }

        public static string GetProductsInRange(ProductShopContext context)
        {
            var products = context.Products.
                Where(p => p.Price >= 500 && p.Price <= 1000)
                .Select(p => new ProductInRangeDto()
                {
                    Name = p.Name,
                    Price = p.Price,
                    Buyer = p.Buyer.FirstName + ' ' + p.Buyer.LastName
                })
                .OrderBy(p => p.Price)
                .Take(10)
                .ToList();

            return SerializeToXml(products, "Products");
        }

        public static string GetSoldProducts(ProductShopContext context)
        {
            var sellers = context.Users
                .Where(u => u.ProductsSold.Any(p => p.Buyer != null))
                .OrderBy(u => u.LastName)
                .ThenBy(u => u.FirstName)
                .Select(u => new SellerDto()
                {
                    FirstName = u.FirstName,
                    LastName = u.LastName,
                    SoldProducts = u.ProductsSold
                    .Select(p => Mapper.Map<SoldProductDto>(p))
                    .ToList()
                })
                .Take(5)
                .ToList();

            return SerializeToXml(sellers, "Users");
        }

        public static string GetCategoriesByProductsCount(ProductShopContext context)
        {
            var categories = context.Categories
                .Select(c => new CategoryDto()
                {
                    Name = c.Name,
                    Count = c.CategoryProducts.Count,
                    AveragePrice = c.CategoryProducts.Average(p => p.Product.Price),
                    TotalRevenue = c.CategoryProducts.Sum(p => p.Product.Price)
                })
                .OrderByDescending(p => p.Count)
                .ThenBy(p => p.TotalRevenue)
                .ToList();

            return SerializeToXml(categories, "Categories");
        }

        public static string GetUsersWithProducts(ProductShopContext context)
        {
            var users = context.Users
                .Where(u => u.ProductsSold.Any())
                .Select(u => new UserByProductDto()
                {
                    FirstName = u.FirstName,
                    LastName = u.LastName,
                    Age = u.Age,
                    SoldProducts = new SoldProductsDto()
                    {
                        Count = u.ProductsSold.Count,
                        Products = u.ProductsSold
                            .Select(p => new SoldProductDto()
                            {
                                Name = p.Name,
                                Price = p.Price
                            })
                            .OrderByDescending(p => p.Price)
                            .ToList()
                    }
                })
                .OrderByDescending(u => u.SoldProducts.Count)
                .Take(10)
                .ToList();

            UserStatisticsDto userStatistics = new UserStatisticsDto()
            {
                Count = context.Users.Count(u => u.ProductsSold.Any()),
                Users = users
            };

            return SerializeToXml(userStatistics, "Users");
        }

        private static string SerializeToXml<T>(T obj, string rootName)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(T), new XmlRootAttribute(rootName));

            var namespaces = new XmlSerializerNamespaces(new XmlQualifiedName[]
            {
               new XmlQualifiedName("", "")
            });

            StringBuilder sb = new StringBuilder();
            serializer.Serialize(new StringWriter(sb), obj, namespaces);

            return sb.ToString().TrimEnd();
        }
    }
}