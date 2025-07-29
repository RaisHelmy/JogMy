using Microsoft.AspNetCore.Identity;

namespace JogMy.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string? FullName { get; set; }
    }
}