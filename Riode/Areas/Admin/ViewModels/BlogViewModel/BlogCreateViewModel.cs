using Riode.Models;

namespace Riode.Areas.Admin.ViewModels.BlogViewModel;
public class BlogCreateViewModel
{
    public string Title { get; set; } = null!;
    public string Description { get; set; } = null!;
    public string Author { get; set; } = null!;
    public string Content { get; set; } = null!;
    public string FamousWord { get; set; } = null!;
    public string AuthorComment { get; set; } = null!;
    public bool isDeleted { get; set; }
    public ICollection<BlogImage> Images { get; set; } = null!;
    public ICollection<IFormFile> BlogImages { get; set; } = null!;
    public ICollection<BlogTopic>? Topics { get; set; } 
    public ICollection<string> BlogTopics { get; set; } = null!;
    public DateTime CreatedDate { get; set; }
    public DateTime UpdatedDate { get; set; }
}
