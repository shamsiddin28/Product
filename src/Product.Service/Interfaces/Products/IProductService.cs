using Microsoft.AspNetCore.Http;
using Product.Domain.Configurations;
using Product.Service.DTOs.Products;
using Product.Service.ViewModels.ProductViewModels;

namespace Product.Service.Interfaces.Products
{
    public interface IProductService
    {
        string GenerateOTP();
        Task<bool> RemoveAsync(long id);
        Task<bool> DeleteVideoAsync(long productId);
        Task<byte[]> DownloadAsync(string videoFilePath);
        Task<byte[]> DownloadOnStaticFileAsync(string videoFilePath);
        Task<ProductViewModel> RetrieveByIdAsync(long id);
        Task<ProductViewModel> RetrieveBySortNumberAsync(long sortNumber);
        Task<List<ProductViewModel>> RetrieveAllProductsAsync();
        Task<bool> UpdateVideoAsync(long id, IFormFile formFile);
        Task<ProductViewModel> CreateAsync(ProductForCreationDto dto);
        Task<List<ProductViewModel>> SearchByProperty(string searchTerm);
        Task<ProductViewModel> UpdateAsync(long id, ProductForUpdateDto dto);
        Task<IEnumerable<ProductViewModel>> RetrieveAllAsync(PaginationParams @params);
    }
}
