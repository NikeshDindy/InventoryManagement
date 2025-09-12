using InventoryManagement.Models;
using InventoryManagement.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace InventoryManagement.Controllers
{
    public class HomeController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<ApplicationUser> _userManager;

        public HomeController(IUnitOfWork unitOfWork, UserManager<ApplicationUser> userManager)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
        }
        public IActionResult Index()
        {
            if (User.Identity != null && User.Identity.IsAuthenticated)
            {
                if (User.IsInRole("Admin"))
                    //return View("AdminDashboard");
                    return RedirectToAction("AdminDashboard");

                if (User.IsInRole("Manager"))
                    return RedirectToAction("ManagerDashboard");

                if (User.IsInRole("Staff"))
                    return RedirectToAction("StaffDashboard");

            }

            return View();
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AdminDashboard()
        {
            var userCount = _userManager.Users.Count();  // total registered users
            var categoryCount = (await _unitOfWork.Categories.GetAllAsync()).Count();
            var supplierCount = (await _unitOfWork.Suppliers.GetAllAsync()).Count();
            var reportCount = 7; // you can calculate dynamically later

            ViewBag.UserCount = userCount;
            ViewBag.CategoryCount = categoryCount;
            ViewBag.SupplierCount = supplierCount;
            ViewBag.ReportCount = reportCount;

            return View();
        }

        [Authorize(Roles = "Manager")]
        public IActionResult ManagerDashboard()
        {
            return View();
        }

        [Authorize(Roles = "Staff")]
        public IActionResult StaffDashboard()
        {
            return View();
        }

        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}
