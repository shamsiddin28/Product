namespace Product.Service.ViewModels
{
    public class AccountBaseViewModel
    {
        public long Id { get; set; }

        public string FirstName { get; set; } = string.Empty;

        public string LastName { get; set; } = string.Empty;

        public string PhoneNumber { get; set; } = string.Empty;

        public string ImagePath { get; set; } = string.Empty;
    }
}
