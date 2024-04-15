using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace Product.Service.DTOs.Products
{
    public class ProductForCreationDto
    {
        [Required(ErrorMessage = "Enter a product name!"), MaxLength(40)]
        public string Name { get; set; }

        public string Description { get; set; } = string.Empty;

        [Required(ErrorMessage = "Upload the Video!")]
        public IFormFile VideoData { get; set; }
    }
}
