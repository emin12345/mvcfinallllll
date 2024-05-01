using Riode.Models;
using System.ComponentModel.DataAnnotations;

namespace Riode.Areas.Admin.ViewModels.ProductViewModels;
public class ProductUpdateViewModel
{
    public int? Id { get; set; }
    [Required]
    public string Name { get; set; } = null!;
    [Required]
    public double Price { get; set; }
    public double? OldPrice { get; set; }
    [Range(0, 5, ErrorMessage = "Rating must be between 0 and 5")]
    public int Rating { get; set; }
    [Required]
    public string Description { get; set; }
    [Range(0, 100)]
    public int Discount { get; set; }
    [Required]
    public string Features { get; set; }
    [Required]
    public string Material { get; set; }
    [Required]
    public bool IsStock { get; set; }
    [Required]
    public string RecommendedUse { get; set; }
    public int CategoryId { get; set; }
    public int BrandId { get; set; }
    public ICollection<ProductImage>? ProductImages { get; set; }
    public ICollection<IFormFile>? ProductImage { get; set; }
}
