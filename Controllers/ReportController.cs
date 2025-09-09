using InventoryManagement.Models;
using InventoryManagement.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace InventoryManagement.Controllers
{
    [Authorize(Roles = "Admin,Manager")]
    public class ReportController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public ReportController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IActionResult> StockSummary()
        {
            var products = await _unitOfWork.Products.GetAllAsync();
            return View(products);
        }

        public async Task<IActionResult> LowStock()
        {
            var lowStockProducts = (await _unitOfWork.Products.GetAllAsync())
                .Where(p => p.StockQuantity <= p.LowStockThreshold)
                .ToList();
            return View(lowStockProducts);
        }

        public async Task<IActionResult> TransactionSummary()
        {
            var transactions = await _unitOfWork.Transactions.GetAllAsync();
            return View(transactions);
        }

        public async Task<IActionResult> ProfitLoss()
        {
            var orders = await _unitOfWork.Orders.GetAllAsync();
            decimal totalSales = orders
                .Where(o => o.OrderType.Equals("Sales") && o.Status != "Cancelled")
                .Sum(o => o.TotalAmount);
            decimal totalPurchases = orders
                .Where(o => o.OrderType.Equals("Purchase") && o.Status != "Cancelled")
                .Sum(o => o.TotalAmount);
            decimal profitLoss = totalSales - totalPurchases;

            ViewBag.TotalSales = totalSales;
            ViewBag.TotalPurchases = totalPurchases;
            ViewBag.ProfitLoss = profitLoss;

            return View();
        }


        // Additional Features

        // GET: Report/OrdersByDateRange
        // Fetch orders created within a specified date range
        public async Task<IActionResult> OrdersByDateRange(DateTime? startDate, DateTime? endDate)
        {
            var query = (await _unitOfWork.Orders.GetAllAsync()).AsQueryable();

            if (startDate.HasValue)
                query = query.Where(o => o.CreatedAt >= startDate.Value);

            if (endDate.HasValue)
                query = query.Where(o => o.CreatedAt <= endDate.Value);

            var filteredOrders = query.ToList();

            return View(filteredOrders);
        }

        // GET: Report/TopSellingProducts
        // Returns top N products by quantity sold
        public async Task<IActionResult> TopSellingProducts(int topN = 10)
        {
            var orders = await _unitOfWork.Orders.GetAllAsync();
            var orderDetails = orders.SelectMany(o => o.OrderDetails).ToList();

            var topProducts = orderDetails
                .GroupBy(od => od.Product)
                .Select(g => new
                {
                    Product = g.Key,
                    TotalQuantitySold = g.Sum(od => od.Quantity)
                })
                .OrderByDescending(p => p.TotalQuantitySold)
                .Take(topN)
                .ToList();

            return View(topProducts);
        }

        // GET: Report/InventoryTurnover
        // Calculates inventory turnover ratio based on sales and average inventory
        public async Task<IActionResult> InventoryTurnover()
        {
            var products = await _unitOfWork.Products.GetAllAsync();
            var orders = await _unitOfWork.Orders.GetAllAsync();

            decimal totalCostOfGoodsSold = orders
                .Where(o => o.OrderType == "Sales" && o.Status != "Cancelled")
                .SelectMany(o => o.OrderDetails)
                .Sum(od => od.LineTotal);

            decimal averageInventoryValue = products
                .Average(p => p.UnitPrice * p.StockQuantity);

            decimal turnoverRatio = averageInventoryValue == 0 ? 0 : totalCostOfGoodsSold / averageInventoryValue;

            ViewBag.TurnoverRatio = turnoverRatio;

            return View();
        }
    }
}
