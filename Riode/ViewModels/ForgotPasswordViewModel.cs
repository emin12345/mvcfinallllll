using System.ComponentModel.DataAnnotations;

namespace Riode.ViewModels;

public class ForgotPasswordViewModel
{
    [Required, DataType(DataType.EmailAddress)]
    public string Email { get; set; }
}