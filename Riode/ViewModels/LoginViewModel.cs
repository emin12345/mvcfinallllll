using System.ComponentModel.DataAnnotations;

namespace Riode.ViewModels;
public class LoginViewModel
{
	[Required]
	public string UsernameOrEmail { get; set; }
	[Required, DataType(DataType.Password)]
	public string Password { get; set; }
	public bool RememberMe { get; set; }
}

