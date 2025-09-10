using InventoryManagement.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace InventoryManagement.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            if (User.IsInRole("Admin"))
                return RedirectToAction("AdminDashboard");

            if (User.IsInRole("Manager"))
                return RedirectToAction("ManagerDashboard");

            if (User.IsInRole("Staff"))
                return RedirectToAction("StaffDashboard");

            return RedirectToAction("AccessDenied");
        }

        [Authorize(Roles = "Admin")]
        public IActionResult AdminDashboard()
        {
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
