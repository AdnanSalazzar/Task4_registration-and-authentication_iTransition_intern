using Microsoft.AspNetCore.Identity;

namespace Task4_registration_and_authentication.Models
{
    public class AppUser : IdentityUser
    {
        public string Name { get; set; }
        public bool IsBlocked { get; set; } 
        public DateTime? LastLoginTime { get; set; }
        
    }
}
