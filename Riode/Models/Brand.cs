    namespace Riode.Models
{
    public class Brand
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string? Logo { get; set; } = null!;
        public bool IsDeleted { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }
        public ICollection<Product>? Products { get; set; }
    }
}