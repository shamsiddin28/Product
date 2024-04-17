namespace Product.Service.Exceptions
{
    public class ModelErrorException : Exception
    {
        public string Property { get; set; } = string.Empty;

        public ModelErrorException(string property, string message)
            : base(message)
        {
            this.Property = property;
        }
    }
}
