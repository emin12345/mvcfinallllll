using Microsoft.AspNetCore.Identity;

namespace Riode.Areas.Admin.ViewModels.UserViewModels
{
    public class ChangeRolesViewModel
    {
        public string UserId { get; set; }
        public string UserName { get; set; }
        public IList<string> UserRoles { get; set; }
        public IList<IdentityRole> AllRoles { get; set; }
        public IList<string> SelectedRoles { get; set; }
    }
}
