using Riode.Models;

namespace Riode.Areas.Admin.ViewModels.UserViewModels;
public class UserViewModel
{
	public AppUser User { get; set; }
	public IList<string> Roles { get; set; } = null!;
}
