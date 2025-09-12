using InventoryManagement.Models;

namespace InventoryManagement.ViewModels
{
    public class AccountsViewModel
    {
        public List<ApplicationUser> Admins { get; set; } = new();
        public List<ApplicationUser> Managers { get; set; } = new();
        public List<ApplicationUser> Staffs { get; set; } = new();
    }
}
