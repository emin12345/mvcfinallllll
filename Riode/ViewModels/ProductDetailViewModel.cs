using Riode.Models;

namespace Riode.ViewModels;
public class ProductDetailViewModel
{
	public int Id { get; set; }
	public string Name { get; set; }
	public string Description { get; set; }
	public double Price { get; set; }
	public double? OldPrice { get; set; }
	public int Rating { get; set; }
	public int Discount { get; set; }
	public string Features { get; set; }
	public string Material { get; set; }
	public int CategoryId { get; set; }
	public string CategoryName { get; set; }
	public int BrandId { get; set; }
	public string BrandName { get; set; }
	public string? RecommendedUse { get; set; }
	public ICollection<ProductImage> Images { get; set; }
	public ICollection<Review> Reviews { get; set; }
}
