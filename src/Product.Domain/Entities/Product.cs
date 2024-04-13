using Product.Domain.Commons;

namespace Product.Domain.Entities
{
    public class Product : Auditable
    {
        public string Name { get; set; }
        
        public string Description { get; set; } = string.Empty;
        
        public string VideoData { get; set; }
        
        public int SortNumber { get; set; }
    }
}
