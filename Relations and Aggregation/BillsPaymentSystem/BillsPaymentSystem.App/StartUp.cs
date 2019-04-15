using BillsPaymentSystem.App.Core;
using BillsPaymentSystem.App.Core.Contracts;
using BillsPaymentSystem.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;

namespace BillsPaymentSystem.App
{
    public class StartUp
    {
        public static void Main(string[] args)
        {
            IServiceProvider serviceProvider = ConfigureServices();

            IEngine engine = serviceProvider.GetService<IEngine>();
            engine.Run();
        }

        private static IServiceProvider ConfigureServices()
        {
            var serviceCollection = new ServiceCollection();
            serviceCollection
                .AddTransient<ICommandInterpreter, CommandInterpreter>()
                .AddTransient<IEngine, Engine>()
                .AddLogging(configure => configure.AddDebug())
                .AddDbContext<BillsPaymentSystemContext>(options =>
                {
                    options.UseSqlServer(Configuration.ConnectionString,
                        b => b.MigrationsAssembly("BillsPaymentSystem.Data"))
                        .EnableSensitiveDataLogging();
                });

            var serviceProvider = serviceCollection.BuildServiceProvider();
            return serviceProvider;
        }
    }
}
