using Riode.Models;

namespace Riode.ViewModels
{
    public class BlogViewModel
    {
        public List<Blog> Blogs { get; set; }
        public Blog Blog { get; set; }
        public ICollection<BlogImage> BlogImages { get; set; }
    }
}
