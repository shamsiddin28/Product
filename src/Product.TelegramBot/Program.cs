using Microsoft.AspNetCore.Hosting.Internal;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Product.Data.Interfaces.IRepositories;
using Product.Data.Repositories;
using Product.Domain.Entities;
using Product.Service.Interfaces.Files;
using Product.Service.Interfaces.Products;
using Product.Service.Mappers;
using Product.Service.Services.Files;
using Product.Service.Services.Products;
using Product.TelegramBot.TelegramBotServices;
using IHostingEnvironment = Microsoft.AspNetCore.Hosting.IHostingEnvironment;

namespace Product.TelegramBot
{
    public class Program
    {
        static async Task Main(string[] args)
        {

            #region Register services

            //var connectionStringToServer = "Server=tcp:serenmebelsqldb.database.windows.net,1433;Initial Catalog=serenmebelsqldb;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;Authentication=\"Active Directory Default\";"
            //    ?? throw new InvalidOperationException("Connection string not found!");

            //var connectionString = "Server=DESKTOP-F20JUCJ\\SQLEXPRESS;Database=TestProductDb;TrustServerCertificate=True;MultipleActiveResultSets=True;Trusted_Connection=True;MultipleActiveResultSets=true"
            //    ?? throw new InvalidOperationException("Connection string not found!");

            // Build configuration
            IConfiguration configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .Build();

            var serviceProvider = new ServiceCollection()
                .AddScoped<ITelegramBotService>(provider =>
                {
                    string botToken = "7174140036:AAGHuKCi0Bysxi0xUEyAj2R2DV1na_nNnYo";

                    var productService = provider.GetRequiredService<IProductService>();

                    return new TelegramBotService(botToken, productService);
                })
                .AddScoped<IRepository<TestProduct>, Repository<TestProduct>>()
                .AddScoped<IProductService, ProductService>()
                .AddScoped<IFileService, FileService>()
                .AddSingleton<IHostingEnvironment, HostingEnvironment>()
                .AddAutoMapper(typeof(MappingConfiguration))
                .ConfigureDataAccess(configuration)
                .BuildServiceProvider();

            // Get and start the bot service
            var botService = serviceProvider.GetRequiredService<ITelegramBotService>();
            #endregion

            await botService.StartBotAsync();

        }
    }
}
