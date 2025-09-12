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
        public async Task<IActionResult> ManagerDashboard()
        {
            var orderCount = (await _unitOfWork.Orders.GetAllAsync()).Count();
            var supplierCount = (await _unitOfWork.Suppliers.GetAllAsync()).Count();
            var productCount = (await _unitOfWork.Products.GetAllAsync()).Count();
            var transactionCount = (await _unitOfWork.Transactions.GetAllAsync()).Count();

            ViewBag.OrderCount = orderCount;
            ViewBag.SupplierCount = supplierCount;
            ViewBag.ProductCount = productCount;
            ViewBag.transactionCount = transactionCount;
            return View();
        }

        [Authorize(Roles = "Staff")]
        public async Task<IActionResult> StaffDashboard()
        {
            var orderCount = (await _unitOfWork.Orders.GetAllAsync()).Count();
            var supplierCount = (await _unitOfWork.Suppliers.GetAllAsync()).Count();
            var productCount = (await _unitOfWork.Products.GetAllAsync()).Count();
            var categoryCount = (await _unitOfWork.Categories.GetAllAsync()).Count();

            ViewBag.OrderCount = orderCount;
            ViewBag.SupplierCount = supplierCount;
            ViewBag.ProductCount = productCount;
            ViewBag.CategoryCount = categoryCount;
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> GetLast6MonthsReportData()
        {
            // include Product for UnitPrice
            var transactions = (await _unitOfWork.Transactions.GetAllAsync(t => t.Product)).ToList();

            var now = DateTime.Now;
            var startDate = new DateTime(now.Year, now.Month, 1).AddMonths(-5); // 6 months window start

            var grouped = transactions
                .Where(t => t.Timestamp >= startDate)
                .GroupBy(t => new { t.Timestamp.Year, t.Timestamp.Month, t.TransactionType })
                .Select(g => new
                {
                    g.Key.Year,
                    g.Key.Month,
                    Type = g.Key.TransactionType,
                    TotalRevenue = g.Sum(t => t.Quantity * t.Product.UnitPrice)
                })
                .ToList();

            var labels = new List<string>();
            var sales = new List<decimal>();
            var purchases = new List<decimal>();

            for (int i = 0; i < 6; i++)
            {
                var date = startDate.AddMonths(i);
                labels.Add(date.ToString("MMM"));

                var saleVal = grouped
                    .Where(g => g.Year == date.Year && g.Month == date.Month && g.Type == "OUT")
                    .Sum(g => g.TotalRevenue);

                var purchaseVal = grouped
                    .Where(g => g.Year == date.Year && g.Month == date.Month && g.Type == "IN")
                    .Sum(g => g.TotalRevenue);

                sales.Add(saleVal);
                purchases.Add(purchaseVal);
            }

            return Json(new { labels, sales, purchases });
        }

        [HttpGet]
        public async Task<IActionResult> GetRecentActivity()
        {
            var recentTransactions = (await _unitOfWork.Transactions
                .GetAllAsync(t => t.Product, t => t.PerformedBy))
                .OrderByDescending(t => t.Timestamp)
                .Take(5)
                .Select(t => new
                {
                    timestamp = t.Timestamp,
                    productName = t.Product != null ? t.Product.Name : "Unknown Product",
                    quantity = t.Quantity,
                    type = t.TransactionType,
                    user = t.PerformedBy != null ? t.PerformedBy.UserName : "System",
                    notes = t.Notes ?? ""
                })
                .ToList();

            return Json(recentTransactions);
        }




        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}
