using System.ComponentModel.DataAnnotations;

namespace Riode.Areas.Admin.ViewModels.CategoryViewModels;
public class CategoryCreateViewModel
{
    [Required]
    public string Name { get; set; }
    [Required]
    public IFormFile Image { get; set; }
}