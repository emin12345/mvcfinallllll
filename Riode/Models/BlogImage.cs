namespace Riode.Models
{
    public class BlogImage
    {
        public int Id { get; set; }
        public string Image { get; set; } = null!;
        public int BlogId { get; set; }
        public Blog Blog { get; set; } = null!;
    }
}
