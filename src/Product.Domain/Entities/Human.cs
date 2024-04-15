using Product.Domain.Commons;

namespace Product.Domain.Entities
{
    public class Human : Auditable
    {
        public string FirstName { get; set; } = string.Empty;

        public string LastName { get; set; } = string.Empty;

        public string Image { get; set; } = string.Empty;

        public string PhoneNumber { get; set; } = string.Empty;

        public DateTime BirthDate { get; set; }
    }
}