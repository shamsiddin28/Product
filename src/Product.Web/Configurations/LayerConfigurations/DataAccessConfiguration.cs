using Microsoft.EntityFrameworkCore;
using Product.Data.DbContexts;

namespace Product.Web.Configurations.LayerConfigurations
{
    public static class DataAccessConfiguration
    {
        public static void ConfigureDataAccess(this IServiceCollection services, IConfiguration configuration)
        {
            //AppContext.SetSwitch("SqlServer.EnableLegacyTimestampBehavior", true);
            string connectionString = configuration.GetConnectionString("DefaultConnection");
            services.AddDbContext<ProductDbContext>(options => options.UseSqlServer(connectionString));
        }
    }
}
