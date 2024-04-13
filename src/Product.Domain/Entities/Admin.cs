using Product.Domain.Commons;
using Product.Domain.Enums;
using System.Data;

namespace Product.Domain.Entities
{
    public class Admin : Auditable
    {
        public string FirstName { get; set; } = string.Empty;

        public string LastName { get; set; } = string.Empty;

        public string Image { get; set; } = string.Empty;

        public string PhoneNumber { get; set; } = string.Empty;
        
        public string Address { get; set; } = string.Empty;

        public Role AdminRole { get; set; } = Role.Admin;

        public string PasswordHash { get; set; } = string.Empty;

        public string Salt { get; set; } = string.Empty;
    }
}
