using System.ComponentModel.DataAnnotations;

namespace Riode.Areas.Admin.ViewModels.BrandViewModels;
public class BrandUpdateViewModel
{
    [Required]
    public string Name { get; set; }
    public IFormFile? Logo { get; set; }
}
