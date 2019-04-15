using AutoMapper;
using EmployeesSystem.Core;
using EmployeesSystem.Core.Contracts;
using EmployeesSystem.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace EmployeesSystem
{
    class StartUp
    {
        public static void Main(string[] args)
        {
            IServiceProvider provider = ConfigureServices();

            IEngine engine = new Engine(provider);
            engine.Run();
        }

        private static IServiceProvider ConfigureServices()
        {
            IServiceCollection serviceCollection = new ServiceCollection();

            serviceCollection.AddDbContext<EmployeeSystemDbContext>(db =>
                db.UseSqlServer(@"Server=DESKTOP-07K6OOE\SQLEXPRESS;Database=EmployeesSyetem;Integrated Security=True"));

            serviceCollection.AddTransient<ICommandInterpreter, CommandInterpreter>();
            serviceCollection.AddTransient<Mapper>();

            return serviceCollection.BuildServiceProvider();
        }
    }
}
