namespace Riode.Models;
public class Product
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public double Price { get; set; }
    public double? OldPrice { get; set; }
    public int Rating { get; set; }
    public bool IsDeleted { get; set; }
    public int Discount { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime UpdatedDate { get; set; }
    public bool IsStock { get; set; }
    public int CategoryId { get; set; }
    public Category Category { get; set; } 
    public int BrandId { get; set; }
    public Brand Brand { get; set; } 
    public ICollection<ProductImage>? Images { get; set; }
    public string Material { get; set; }
    public string Features { get; set; }
    public string? RecommendedUse { get; set; }
    public ICollection<Review>? Reviews { get; set; }
}
