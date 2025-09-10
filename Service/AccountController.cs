using InventoryManagement.Models;
using InventoryManagement.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace InventoryManagement.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private ILogger<AccountController> _logger;
        public AccountController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            RoleManager<IdentityRole> roleManager, ILogger<AccountController> logger)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _logger = logger;
        }

        // GET: /Account/Register
        [Authorize(Roles = "Admin")]
        public IActionResult Register()
        {
            return View();
        }

        // POST: /Account/Register
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var userExists = await _userManager.FindByEmailAsync(model.Email);
            if (userExists != null)
            {
                ModelState.AddModelError(string.Empty, "User already exists!");
                return View(model);
            }

            var user = new ApplicationUser
            {
                UserName = model.Email,
                Email = model.Email,
                FullName = model.FullName
            };

            var result = await _userManager.CreateAsync(user, model.Password);

            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                    ModelState.AddModelError(string.Empty, error.Description);

                return View(model);
            }

            // Create role if it doesn't exist
            if (!await _roleManager.RoleExistsAsync(model.Role))
                await _roleManager.CreateAsync(new IdentityRole(model.Role));

            // Assign role to user
            await _userManager.AddToRoleAsync(user, model.Role);

            TempData["Success"] = "User registered successfully!";
            return RedirectToAction("AdminDashboard", "Home");
        }

        // GET: /Account/Login
        public IActionResult Login()
        {
            return View();
        }

        // POST: /Account/Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model, string? returnUrl = null)
        {
            if (!ModelState.IsValid)

                return View(model);

            var result = await _signInManager.PasswordSignInAsync(
                model.Email, model.Password, model.RememberMe, lockoutOnFailure: false);

            if (result.Succeeded)
            {
                var user = await _userManager.FindByEmailAsync(model.Email);
                var roles = await _userManager.GetRolesAsync(user);

                _logger.LogInformation($"✅ User {model.Email} logged in successfully as {string.Join(",", roles)}.");

                // Redirect according to role
                if (roles.Contains("Admin")) {
                    _logger.LogInformation("-----------admin dashborard entering-----------");
                    return RedirectToAction("AdminDashboard", "Home");
                }
                    
                else if (roles.Contains("Manager"))
                    return RedirectToAction("ManagerDashboard", "Home");
                else if (roles.Contains("Staff"))
                    return RedirectToAction("StaffDashboard", "Home");
                else
                    return RedirectToAction("Index", "Home");
            }
            _logger.LogInformation("-----------out the dashborads-----------");
            ModelState.AddModelError(string.Empty, "Invalid login attempt.");
            return View(model);
        }

        // POST: /Account/Logout
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }
    }
}
