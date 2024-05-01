using Riode.Models;

namespace Riode.ViewModels
{
    public class HomeViewModel
    {
        public List<Product> Products { get; set; }
        public List<Category> Categories { get; set; }
        public ICollection<ProductImage> ProductImages { get; set; }
    }
}
