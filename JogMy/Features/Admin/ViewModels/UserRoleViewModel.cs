using JogMy.Models;

namespace JogMy.Features.Admin.ViewModels
{
    public class UserRoleViewModel
    {
        public ApplicationUser User { get; set; } = new();
        public List<string> Roles { get; set; } = new();
    }
}