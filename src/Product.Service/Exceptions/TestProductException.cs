namespace Product.Service.Exceptions
{
    public class TestProductException : Exception
    {
        public int StatusCode { get; set; }

        public TestProductException(int code, string message) : base(message)
        {
            StatusCode = code;
        }
    }
}
