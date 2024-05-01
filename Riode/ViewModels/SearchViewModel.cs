using System.ComponentModel.DataAnnotations;

namespace Riode.ViewModels;

public class SearchViewModel
{
    [Required]
    public string SearchItem { get; set; }
}
