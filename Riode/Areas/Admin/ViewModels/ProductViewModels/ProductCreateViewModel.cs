using Riode.Models;
using System.ComponentModel.DataAnnotations;

namespace Riode.Areas.Admin.ViewModels.ProductViewModels;
public class ProductCreateViewModel
{
    public int Id { get; set; }
    [Required, MaxLength(50)]
    public string Name { get; set; }
    [Required, MaxLength(500)]
    public string Description { get; set; }
    [Required]
    public double Price { get; set; }
    [Range(0, 100)]
    public int Discount { get; set; }
    [Range(0, 5)]
    public int Rating { get; set; }
    public bool IsStock { get; set; }
    public string Material { get; set; }
    public string Features { get; set; }
    public string RecommendedUse { get; set; }
    //public IFormFile Image { get; set; }
    public bool IsDeleted { get; set; }
    public int CategoryId { get; set; }
    public int BrandId { get; set; }
    public ICollection<ProductImage>? ProductImages { get; set; }
    public ICollection<IFormFile>? ProductImage { get; set; }
}

