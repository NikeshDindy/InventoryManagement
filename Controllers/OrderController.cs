using InventoryManagement.Models;
using InventoryManagement.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Threading.Tasks;

namespace InventoryManagement.Controllers
{
    public class OrderController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public OrderController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        // GET: Orders
        public async Task<IActionResult> Index()
        {
            var orders = await _unitOfWork.Orders.GetAllAsync();
            return View(orders);
        }

        // GET: Order/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var order = await _unitOfWork.Orders.GetByIdAsync(id);
            if (order == null)
                return NotFound();
            return View(order);
        }

        // GET: Order/CreatePurchaseOrder
        public IActionResult CreatePurchaseOrder()
        {
            // Render purchase order creation form
            return View();
        }

        // POST: Order/CreatePurchaseOrder
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreatePurchaseOrder(Order order)
        {
            if (ModelState.IsValid && order.OrderType == "Purchase")
            {
                await _unitOfWork.Orders.AddAsync(order);
                await _unitOfWork.CompleteAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(order);
        }

        // GET: Order/CreateSalesOrder
        public IActionResult CreateSalesOrder()
        {
            // Render sales order creation form
            return View();
        }

        // POST: Order/CreateSalesOrder
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateSalesOrder(Order order)
        {
            if (ModelState.IsValid && order.OrderType == "Sales")
            {
                await _unitOfWork.Orders.AddAsync(order);
                await _unitOfWork.CompleteAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(order);
        }

        // POST: Order/ApproveOrder/5
        [Authorize(Roles = "Manager")]
        [HttpPost]
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

        // POST: Order/CancelOrder/5
        [HttpPost]
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
    }
}
