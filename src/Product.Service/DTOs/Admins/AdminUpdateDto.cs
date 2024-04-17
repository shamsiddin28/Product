using Microsoft.AspNetCore.Http;
using Product.Domain.Entities;
using Product.Service.Commons.Attributes;
using System.ComponentModel.DataAnnotations;

namespace Product.Service.DTOs.Admins
{
    public class AdminUpdateDto
    {
        [Required(ErrorMessage = "First Name Required")]
        public string FirstName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Last Name Required")]
        public string LastName { get; set; } = string.Empty;

        [AllowedFiles(new string[] { ".jpg", ".png", ".jpeg", ".svg", ".webp" }), MaxFile(2)]
        public IFormFile? Image { get; set; }
        public string? ImagePath { get; set; }

        [Required(ErrorMessage = "Phone Number Required")]
        [Phone(ErrorMessage = "The phone number was entered incorrectly")]
        public string PhoneNumber { get; set; } = string.Empty;

        public DateTime BirthDate { get; set; }

        [Required(ErrorMessage = "Address Required")]
        public string Address { get; set; } = string.Empty;

        public static implicit operator Admin(AdminUpdateDto dto)
        {
            return new Admin()
            {
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                Image = dto.ImagePath,
                PhoneNumber = dto.PhoneNumber,
                Address = dto.Address,
                BirthDate = dto.BirthDate
            };
        }
    }
}
