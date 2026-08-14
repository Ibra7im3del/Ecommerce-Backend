using Microsoft.AspNetCore.Identity;

namespace EcommerceApi.Entities
{
    public class ApplicationUser : IdentityUser
    {
        public string FullName { get; set; } = string.Empty; 
    }
}
