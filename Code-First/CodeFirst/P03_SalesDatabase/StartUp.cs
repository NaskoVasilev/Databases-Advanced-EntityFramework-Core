using P03_SalesDatabase.Data;
using P03_SalesDatabase.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace P03_SalesDatabase
{
    class StartUp
    {
        static void Main(string[] args)
        {
            SeedDatabase();
        }

        private static void SeedDatabase()
        {
            using (var context = new SalesContext())
            {
                SeedProducts(context);
                SeedCustomers(context);
                SeedStore(context);
                context.SaveChanges();

                SeedSales(context);
                context.SaveChanges();
            }
        }

        private static void SeedSales(SalesContext context)
        {
            List<Product> Products = context.Products.
                Take(20).
                ToList();

            List<Customer> Customers = context.Customers.
               Take(20).
               ToList();

            List<Store> Stores = context.Stores.
               Take(5).
               ToList();


            for (int i = 1; i <= 40; i++)
            {
                context.Sales.Add(new Sale()
                {
                    Date = DateTime.Now,
                    Product = Products[i % Products.Count],
                    Customer = Customers[i % Customers.Count],
                    Store = Stores[i % Stores.Count],
                });
            }
        }

        private static void SeedStore(SalesContext context)
        {
            for (int i = 1; i <= 5; i++)
            {
                context.Stores.Add(new Store()
                {
                    Name = "Store" + i
                });
            }
        }

        private static void SeedCustomers(SalesContext context)
        {

            for (int i = 1; i <= 20; i++)
            {
                context.Customers.Add(new Customer()
                {
                    Name = "Product" + i,
                    Email = "person" + i + "@abv.bg",
                    CreditCardNumber = new string(i.ToString()[0], 10)
                });
            }
        }

        private static void SeedProducts(SalesContext context)
        {
            for (int i = 1; i <= 20; i++)
            {
                context.Products.Add(new Product()
                {
                    Name = "Product" + i,
                    Quantity = i,
                    Price = i
                });
            }
        }
    }
}
