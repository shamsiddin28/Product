using Product.Domain.Entities;

namespace Product.Service.ViewModels.ProductViewModels
{
    public class ProductViewModel
    {
        public long Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; } = string.Empty;

        public int SortNumber { get; set; }

        public string VideoData { get; set; }

        public DateTime CreatedAt { get; set; } = default!;

        public DateTime? UpdatedAt { get; set; }

        public static implicit operator ProductViewModel(TestProduct model)
        {
            return new ProductViewModel()
            {
                Id = model.Id,
                Name = model.Name,
                Description = model.Description,
                SortNumber = model.SortNumber,
                VideoData = model.VideoData,
                CreatedAt = model.CreatedAt,
                UpdatedAt = model.UpdatedAt
            };
        }
    }
}
