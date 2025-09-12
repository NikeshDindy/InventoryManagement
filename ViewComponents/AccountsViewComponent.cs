using InventoryManagement.Models;
using InventoryManagement.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace InventoryManagement.ViewComponents
{
    public class AccountsViewComponent : ViewComponent
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public AccountsViewComponent(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var users = _userManager.Users.ToList();

            var admins = new List<ApplicationUser>();
            var managers = new List<ApplicationUser>();
            var staffs = new List<ApplicationUser>();

            foreach (var user in users)
            {
                if (await _userManager.IsInRoleAsync(user, "Admin"))
                    admins.Add(user);
                else if (await _userManager.IsInRoleAsync(user, "Manager"))
                    managers.Add(user);
                else if (await _userManager.IsInRoleAsync(user, "Staff"))
                    staffs.Add(user);
            }

            var model = new AccountsViewModel
            {
                Admins = admins,
                Managers = managers,
                Staffs = staffs
            };

            return View(model);
        }
    }
}
