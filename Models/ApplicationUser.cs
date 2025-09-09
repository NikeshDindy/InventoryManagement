using Microsoft.AspNetCore.Identity;

namespace InventoryManagement.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string FullName { get; set; }
    }
}
