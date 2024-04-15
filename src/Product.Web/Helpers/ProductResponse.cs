namespace Product.Web.Helpers
{
    public class ProductResponse
    {
        public int Code { get; set; } = 200;
        public string Message { get; set; }
        public object Data { get; set; }
    }
}
