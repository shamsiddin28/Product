using Microsoft.Extensions.DependencyInjection;
using Product.Data.Interfaces.IRepositories;
using Product.Data.Repositories;
using Product.Domain.Entities;
using Product.Service.Interfaces.Accounts;
using Product.Service.Interfaces.Admins;
using Product.Service.Interfaces.Commons;
using Product.Service.Interfaces.Files;
using Product.Service.Interfaces.Products;
using Product.Service.Mappers;
using Product.Service.Services.Accounts;
using Product.Service.Services.Admins;
using Product.Service.Services.Commons;
using Product.Service.Services.Files;
using Product.Service.Services.Products;
using Product.Web.Areas;

namespace Product.Web.Configurations.LayerConfigurations
{
    public static class ServiceLayerConfiguration
    {
        public static void AddService(this IServiceCollection services)
        {
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IFileService, FileService>();
            services.AddScoped<IAdminService, AdminService>();
            services.AddScoped<IProductService, ProductService>();
            services.AddScoped<IAccountService, AccountService>();
            services.AddScoped<IIdentityService, IdentityService>();
            services.AddScoped<IRepository<Admin>, Repository<Admin>>();
            services.AddScoped<IRepository<TestProduct>, Repository<TestProduct>>();

            services.AddMemoryCache();
            services.AddHttpContextAccessor();
            services.AddAutoMapper(typeof(MappingConfiguration));
        }
    }
}
