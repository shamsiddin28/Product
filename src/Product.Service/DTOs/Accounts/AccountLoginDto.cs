using Product.Service.Commons.Attributes;
using System.ComponentModel.DataAnnotations;

namespace Product.Service.DTOs.Accounts
{
    public class AccountLoginDto
    {
        [Required(ErrorMessage = "Enter a phone number!")]
        [PhoneNumber]
        public string PhoneNumber { get; set; } = string.Empty;

        [Required(ErrorMessage = "Enter a password!")]
        [StrongPassword]
        public string Password { get; set; } = string.Empty;
    }
}
