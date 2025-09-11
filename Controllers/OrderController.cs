using InventoryManagement.Models;
using InventoryManagement.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace InventoryManagement.Controllers
{
    [Authorize(Roles = "Admin,Manager,Staff")]
    public class OrderController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public OrderController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        // =======================
        // VIEW ORDERS
        // =======================
        [Authorize(Roles = "Admin,Manager,Staff")]
        public async Task<IActionResult> Index()
        {
            var orders = await _unitOfWork.Orders.GetAllAsync();
            return View(orders);
        }

        [Authorize(Roles = "Admin,Manager,Staff")]
        public async Task<IActionResult> Details(int id)
        {
            var order = await _unitOfWork.Orders.GetByIdAsync(id);
            if (order == null)
                return NotFound();
            return View(order);
        }

        // =======================
        // CREATE PURCHASE ORDER (Manager/Admin)
        // =======================
        [Authorize(Roles = "Manager,Admin")]
        public async Task<IActionResult> CreatePurchaseOrder()
        {
            var suppliers = await _unitOfWork.Suppliers.GetAllAsync();
            ViewBag.Suppliers = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(suppliers, "SupplierId", "Name");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Manager,Admin")]
        public async Task<IActionResult> CreatePurchaseOrder(Order order)
        {
            if (ModelState.IsValid)
            {
                order.OrderType = "Purchase";
                order.Status = "Pending";
                await _unitOfWork.Orders.AddAsync(order);
                await _unitOfWork.CompleteAsync();
                return RedirectToAction(nameof(Index));
            }

            var suppliers = await _unitOfWork.Suppliers.GetAllAsync();
            ViewBag.Suppliers = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(suppliers, "SupplierId", "Name");
            return View(order);
        }

        // =======================
        // CREATE SALES ORDER (Staff/Manager/Admin)
        // =======================
        [Authorize(Roles = "Staff,Manager,Admin")]
        public IActionResult CreateSalesOrder()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Staff,Manager,Admin")]
        public async Task<IActionResult> CreateSalesOrder(Order order)
        {
            if (ModelState.IsValid)
            {
                order.OrderType = "Sales";
                order.Status = "Pending";
                await _unitOfWork.Orders.AddAsync(order);
                await _unitOfWork.CompleteAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(order);
        }

        // =======================
        // APPROVE / CANCEL ORDERS (Manager/Admin)
        // =======================
        [HttpPost]
        [Authorize(Roles = "Manager,Admin")]
        public async Task<IActionResult> ApproveOrder(int id)
        {
            var order = await _unitOfWork.Orders.GetByIdAsync(id);
            if (order == null)
                return NotFound();

            order.Status = "Approved";
            _unitOfWork.Orders.Update(order);
            await _unitOfWork.CompleteAsync();

            return RedirectToAction(nameof(Details), new { id });
        }

        [HttpPost]
        [Authorize(Roles = "Manager,Admin")]
        public async Task<IActionResult> CancelOrder(int id)
        {
            var order = await _unitOfWork.Orders.GetByIdAsync(id);
            if (order == null)
                return NotFound();

            order.Status = "Cancelled";
            _unitOfWork.Orders.Update(order);
            await _unitOfWork.CompleteAsync();

            return RedirectToAction(nameof(Details), new { id });
        }

        // =======================
        // DELETE ORDER (Admin only)
        // =======================
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var order = await _unitOfWork.Orders.GetByIdAsync(id);
            if (order == null)
                return NotFound();

            _unitOfWork.Orders.Remove(order);
            await _unitOfWork.CompleteAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}
