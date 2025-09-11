using InventoryManagement.Models;
using InventoryManagement.Models.ViewModel.Orders;
using InventoryManagement.Repositories;
using InventoryManagement.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

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

            var orderDtos = orders.Select(o => new OrderDto
            {
                OrderId = o.OrderId,
                OrderType = o.OrderType,
                Status = o.Status,
                TotalAmount = o.TotalAmount,
                CreatedAt = o.CreatedAt,
                SupplierName = o.Supplier?.Name
            }).ToList();

            return View(orderDtos);
        }

        [Authorize(Roles = "Admin,Manager,Staff")]
        public async Task<IActionResult> Details(int id)
        {
            var order = await _unitOfWork.Orders.GetByIdAsync(id);
            if (order == null)
                return NotFound();

            var orderDto = new OrderDto
            {
                OrderId = order.OrderId,
                OrderType = order.OrderType,
                Status = order.Status,
                TotalAmount = order.TotalAmount,
                CreatedAt = order.CreatedAt,
                SupplierName = order.Supplier?.Name
            };

            return View(orderDto);
        }

        // =======================
        // CREATE PURCHASE ORDER (Manager/Admin)
        // =======================
        [Authorize(Roles = "Manager,Admin")]
        public async Task<IActionResult> CreatePurchaseOrder()
        {
            var suppliers = await _unitOfWork.Suppliers.GetAllAsync();
            ViewBag.Suppliers = new SelectList(suppliers, "SupplierId", "Name");
            return View(new CreatePurchaseOrderDto());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Manager,Admin")]
        public async Task<IActionResult> CreatePurchaseOrder(CreatePurchaseOrderDto dto)
        {
            if (ModelState.IsValid)
            {
                var order = new Order
                {
                    SupplierId = dto.SupplierId,
                    TotalAmount = dto.TotalAmount,
                    OrderType = "Purchase",
                    Status = "Pending",
                    CreatedAt = DateTime.UtcNow
                };

                await _unitOfWork.Orders.AddAsync(order);
                await _unitOfWork.CompleteAsync();
                return RedirectToAction(nameof(Index));
            }

            var suppliers = await _unitOfWork.Suppliers.GetAllAsync();
            ViewBag.Suppliers = new SelectList(suppliers, "SupplierId", "Name");
            return View(dto);
        }

        // =======================
        // CREATE SALES ORDER (Staff/Manager/Admin)
        // =======================
        [Authorize(Roles = "Staff,Manager,Admin")]
        public IActionResult CreateSalesOrder()
        {
            return View(new CreateSalesOrderDto());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Staff,Manager,Admin")]
        public async Task<IActionResult> CreateSalesOrder(CreateSalesOrderDto dto)
        {
            if (ModelState.IsValid)
            {
                var order = new Order
                {
                    TotalAmount = dto.TotalAmount,
                    OrderType = "Sales",
                    Status = "Pending",
                    CreatedAt = DateTime.UtcNow
                };

                await _unitOfWork.Orders.AddAsync(order);
                await _unitOfWork.CompleteAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(dto);
        }

        // =======================
        // APPROVE / CANCEL ORDERS (Manager/Admin)
        // =======================
        [HttpPost]
        [Authorize(Roles = "Manager,Admin")]
        public async Task<IActionResult> ApproveOrder(UpdateOrderStatusDto dto)
        {
            var order = await _unitOfWork.Orders.GetByIdAsync(dto.OrderId);
            if (order == null) return NotFound();

            order.Status = "Approved";
            _unitOfWork.Orders.Update(order);
            await _unitOfWork.CompleteAsync();

            return RedirectToAction(nameof(Details), new { id = dto.OrderId });
        }

        [HttpPost]
        [Authorize(Roles = "Manager,Admin")]
        public async Task<IActionResult> CancelOrder(UpdateOrderStatusDto dto)
        {
            var order = await _unitOfWork.Orders.GetByIdAsync(dto.OrderId);
            if (order == null) return NotFound();

            order.Status = "Cancelled";
            _unitOfWork.Orders.Update(order);
            await _unitOfWork.CompleteAsync();

            return RedirectToAction(nameof(Details), new { id = dto.OrderId });
        }

        // =======================
        // DELETE ORDER (Admin only)
        // =======================
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var order = await _unitOfWork.Orders.GetByIdAsync(id);
            if (order == null) return NotFound();

            _unitOfWork.Orders.Remove(order);
            await _unitOfWork.CompleteAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}
