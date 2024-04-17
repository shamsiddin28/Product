using Product.Domain.Enums;

namespace Product.Domain.Entities
{
    public class Admin : Human
    {
        public string Address { get; set; } = string.Empty;

        public Role AdminRole { get; set; } = Role.Admin;

        public string PasswordHash { get; set; } = string.Empty;

        public string Salt { get; set; } = string.Empty;
    }
}
