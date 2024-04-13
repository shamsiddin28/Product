using Product.Service.DTOs.Accounts;
using System.ComponentModel.DataAnnotations;

namespace Product.Service.DTOs.Admins
{
    public class AdminRegisterDto : AccountRegisterDto
    {
        [Required(ErrorMessage = "Please enter the address of the admin.")]
        public string Address { get; set; } = string.Empty;
    }
}
}
