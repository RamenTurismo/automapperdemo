using AutoMapper;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Reflection;
using System.Threading.Tasks;

namespace AutoMapperDemo
{
    /*
     * AutoMapper
     * https://docs.automapper.org/en/stable
     */
    public static class Program
    {
        public static async Task Main(string[] args)
        {
            ServiceCollection servicesCollection = new();

            servicesCollection.AddLogging(builder =>
            {
                builder.SetMinimumLevel(LogLevel.Information);
                builder.AddDebug();
                builder.AddConsole();
            });

            servicesCollection.AddAutoMapper(Assembly.GetExecutingAssembly());

            SqliteConnection connection = new("Data Source=:memory:");
            await connection.OpenAsync();

            servicesCollection.AddDbContext<DemoDbContext>(builder =>
                {
                    builder.EnableDetailedErrors();
                    builder.EnableSensitiveDataLogging();
                    builder.UseSqlite(connection);
                },
                optionsLifetime: ServiceLifetime.Singleton);

            servicesCollection.AddScoped<DatabaseTestClass>();

            await using ServiceProvider services = servicesCollection.BuildServiceProvider();
            await InitializeDemoAsync(services);

            services.GetRequiredService<IConfigurationProvider>().AssertConfigurationIsValid();

            using (IServiceScope scope = services.CreateScope())
            {
                var queryableDemo = scope.ServiceProvider.GetRequiredService<DatabaseTestClass>();
                    
                await queryableDemo.WithClassicSelect();
                await queryableDemo.WithProjectTo();
                await queryableDemo.WithMapperMap();
            }
            
            Console.Read();
        }

        private static async Task InitializeDemoAsync(IServiceProvider provider)
        {
            using IServiceScope scope = provider.CreateScope();
            DemoDbContext dbContext = scope.ServiceProvider.GetRequiredService<DemoDbContext>();
            await dbContext.Database.EnsureCreatedAsync();
        }
    }
}