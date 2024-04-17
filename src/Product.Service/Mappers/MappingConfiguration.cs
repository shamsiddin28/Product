using AutoMapper;
using Product.Domain.Entities;
using Product.Service.DTOs.Admins;
using Product.Service.DTOs.Products;
using Product.Service.ViewModels.ProductViewModels;

namespace Product.Service.Mappers
{
    public class MappingConfiguration : Profile
    {
        public MappingConfiguration()
        {
            // Admin
            CreateMap<Admin, AdminRegisterDto>().ReverseMap();

            // Product
            CreateMap<TestProduct, ProductForCreationDto>().ReverseMap();
            CreateMap<TestProduct, ProductForUpdateDto>().ReverseMap();
            CreateMap<TestProduct, ProductViewModel>().ReverseMap();
        }
    }
}
