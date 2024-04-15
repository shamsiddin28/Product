using Microsoft.AspNetCore.Http;
using Product.Domain.Entities;
using System.ComponentModel.DataAnnotations;

namespace Product.Service.DTOs.Products
{
    public class ProductForUpdateDto
    {
        [Required(ErrorMessage = "Enter a product name!"), MaxLength(40)]
        public string Name { get; set; }

        public string Description { get; set; } = string.Empty;

        [Required(ErrorMessage = "Upload the Video!")]
        public IFormFile VideoData { get; set; }

        public string VideoFilePath { get; set; }

        public static implicit operator TestProduct(ProductForUpdateDto dto)
        {
            return new TestProduct()
            {
                Name = dto.Name,
                Description = dto.Description,
                VideoData = dto.VideoFilePath,
            };
        }
    }
}
