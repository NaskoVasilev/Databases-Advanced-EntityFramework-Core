using AutoMapper;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using ProductShop.Data;
using ProductShop.Dtos;
using ProductShop.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ProductShop
{
    public class StartUp
    {
        public static void Main(string[] args)
        {
            //const string path = "./Datasets/";
            //const string fileName = "categories-products.json";
            //string filePath = path + fileName;
            //string json = File.ReadAllText(filePath);

            Mapper.Initialize(opt =>
            {
                opt.AddProfile<ProductShopProfile>();
            });

            using (ProductShopContext context = new ProductShopContext())
            {
                string result = GetUsersWithProducts(context);
                Console.WriteLine(result);
            }
        }

        public static string ImportUsers(ProductShopContext context, string inputJson)
        {
            List<User> users = JsonConvert.DeserializeObject<List<User>>(inputJson);
            context.Users.AddRange(users);
            context.SaveChanges();

            return $"Successfully imported {users.Count}";
        }

        public static string ImportProducts(ProductShopContext context, string inputJson)
        {
            List<Product> products = JsonConvert.DeserializeObject<List<Product>>(inputJson);
            context.Products.AddRange(products);
            context.SaveChanges();

            return $"Successfully imported {products.Count}";
        }

        public static string ImportCategories(ProductShopContext context, string inputJson)
        {
            List<Category> categories = JsonConvert.DeserializeObject<List<Category>>(inputJson)
                .Where(c => c.Name != null)
                .ToList();
            context.Categories.AddRange(categories);
            context.SaveChanges();

            return $"Successfully imported {categories.Count}";
        }

        public static string ImportCategoryProducts(ProductShopContext context, string inputJson)
        {
            List<CategoryProduct> categoryProducts = JsonConvert.DeserializeObject<List<CategoryProduct>>(inputJson);

            context.CategoryProducts.AddRange(categoryProducts);
            context.SaveChanges();

            return $"Successfully imported {categoryProducts.Count}";
        }

        public static string GetProductsInRange(ProductShopContext context)
        {
            List<ProductInRangeDto> productsInRange = context.Products
                .Where(p => p.Price >= 500 && p.Price <= 1000)
                .OrderBy(p => p.Price)
                .Select(p => new ProductInRangeDto()
                {
                    Name = p.Name,
                    Price = p.Price,
                    Seller = p.Seller.FirstName + ' ' + p.Seller.LastName
                })
                .ToList();

            string result = JsonConvert.SerializeObject(productsInRange);
            return result;
        }

        public static string GetSoldProducts(ProductShopContext context)
        {
            var sellers = context.Users
                .Where(u => u.ProductsSold.Any(p => p.BuyerId != null))
                .OrderBy(u => u.LastName)
                .ThenBy(u => u.FirstName)
                .Select(u => new UserSoldProductsDto()
                {
                    FirstName = u.FirstName,
                    LastName = u.LastName,
                    SoldProducts = u.ProductsSold
                        .Where(p => p.BuyerId != null)
                        .Select(p => new ProductBuyerDto()
                        {
                            Name = p.Name,
                            Price = p.Price,
                            BuyerFirstName = p.Buyer.FirstName,
                            BuyerLastName = p.Buyer.LastName
                        })
                        .ToList()
                })
                .ToList();

            return SerializeObjectUsingCamelCase(sellers);
        }

        public static string GetCategoriesByProductsCount(ProductShopContext context)
        {
            var categories = context.Categories
                .Select(c => new CategoryStatisticsDto()
                {
                    Category = c.Name,
                    ProductsCount = c.CategoryProducts.Count,
                    AveragePrice = $"{c.CategoryProducts.Average(p => p.Product.Price):F2}",
                    TotalRevenue = $"{c.CategoryProducts.Sum(p => p.Product.Price):F2}"
                })
                .OrderByDescending(c => c.ProductsCount)
                .ToList();

            return SerializeObjectUsingCamelCase(categories);
        }

        public static string GetUsersWithProducts(ProductShopContext context)
        {
            var users = context.Users
                .Where(u => u.ProductsSold.Any(p => p.BuyerId != null))
                .OrderByDescending(u => u.ProductsSold.Count(p => p.BuyerId != null))
                .Select(u => new SellerProductsDto()
                {
                    FirstName = u.FirstName,
                    LastName = u.LastName,
                    Age = u.Age,
                    SoldProducts = new SoldProductsDto()
                    {
                        Count = u.ProductsSold.Count(ps => ps.BuyerId != null),
                        Products = u.ProductsSold
                           .Where(ps => ps.BuyerId != null)
                           .Select(up => Mapper.Map<SoldProductDto>(up))
                           .ToList()
                    }
                })
                .ToList();

            var usersProducts = new
            {
                UsersCount = users.Count,
                Users = users
            };

            DefaultContractResolver contractResolver = new DefaultContractResolver()
            {
                NamingStrategy = new CamelCaseNamingStrategy()
            };

            string serialized = JsonConvert.SerializeObject(usersProducts, new JsonSerializerSettings()
            {
                ContractResolver = contractResolver,
                NullValueHandling = NullValueHandling.Ignore,
                Formatting = Formatting.Indented
            });
            return serialized;
        }

        private static string SerializeObjectUsingCamelCase<T>(T obj)
        {
            DefaultContractResolver contractResolver = new DefaultContractResolver()
            {
                NamingStrategy = new CamelCaseNamingStrategy()
            };

            string serialized = JsonConvert.SerializeObject(obj, new JsonSerializerSettings()
            {
                ContractResolver = contractResolver
            });

            return serialized;
        }
    }
}