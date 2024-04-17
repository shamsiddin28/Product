using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Product.Data.DbContexts;

namespace Product.TelegramBot
{
    public static class DataAccessConfiguration
    {
        public static IServiceCollection ConfigureDataAccess(this IServiceCollection services, IConfiguration configuration)
        {
            //AppContext.SetSwitch("SqlServer.EnableLegacyTimestampBehavior", true);
            string connectionString = configuration.GetConnectionString("DefaultConnection");
            services.AddDbContext<ProductDbContext>(options => options.UseSqlServer(connectionString));
            return services;
        }
    }
}
