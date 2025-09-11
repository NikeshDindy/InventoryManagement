using InventoryManagement.Models;
using InventoryManagement.Repositories;
using InventoryManagement.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
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

        // details
        //[Authorize(Roles = "Admin,Manager,Staff")]
        //public async Task<IActionResult> Details(int id)
        //{
        //    var order = await _unitOfWork.Orders.GetByIdAsync(
        //                    id,
        //                    o => o.OrderId,  // explicitly tell EF which property is the key
        //                    o => o.OrderDetails,
        //                    o => o.InventoryTransactions,
        //                    o => o.CreatedBy,
        //                    o => o.Supplier
        //    );

        //    if (order == null)
        //        return NotFound();

        //    // Map to DTO
        //    var dto = new OrderDetailDto
        //    {
        //        OrderId = order.OrderId,
        //        OrderNumber = order.OrderNumber,
        //        OrderType = order.OrderType,
        //        Status = order.Status,
        //        TotalAmount = order.TotalAmount,
        //        CreatedAt = order.CreatedAt,
        //        CreatedByUserId = order.CreatedByUserId,
        //        CreatedByName = order.CreatedBy?.UserName ?? "",
        //        SupplierName = order.Supplier?.Name ?? "",
        //        OrderDetails = order.OrderDetails ?? new List<OrderDetail>(),
        //        InventoryTransactions = order.InventoryTransactions ?? new List<InventoryTransaction>()
        //    };

        //    return View(dto);
        //}
        // =======================
        // CREATE PURCHASE ORDER
        // =======================
        [Authorize(Roles = "Manager,Admin")]
        public async Task<IActionResult> CreatePurchaseOrder()
        {
            var suppliers = await _unitOfWork.Suppliers.GetAllAsync();
            ViewBag.Suppliers = new SelectList(suppliers, "SupplierId", "Name");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Manager,Admin")]
        public async Task<IActionResult> CreatePurchaseOrder(PurchaseOrderDto dto)
        {
            if (!ModelState.IsValid)
            {
                var suppliers = await _unitOfWork.Suppliers.GetAllAsync();
                ViewBag.Suppliers = new SelectList(suppliers, "SupplierId", "Name", dto.SupplierId);
                return View(dto);
            }

            // Map DTO to actual Order model
            var order = new Order
            {
                OrderNumber = dto.OrderNumber,
                SupplierId = dto.SupplierId,
                TotalAmount = dto.TotalAmount,
                OrderType = "Purchase",
                Status = "Pending",
                CreatedByUserId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? string.Empty,
                OrderDetails = new List<OrderDetail>(),
                InventoryTransactions = new List<InventoryTransaction>()
            };

            await _unitOfWork.Orders.AddAsync(order);
            await _unitOfWork.CompleteAsync();

            return RedirectToAction(nameof(Index));
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
        //[HttpPost]
        [Authorize(Roles = "Manager,Admin")]
        public async Task<IActionResult> ApproveOrder(int id)
        {
            var order = await _unitOfWork.Orders.GetByIdAsync(id);
            if (order == null) return NotFound();

            order.Status = "Approved";
            _unitOfWork.Orders.Update(order);
            await _unitOfWork.CompleteAsync();

            //return RedirectToAction(nameof(Details), new { id });
            return RedirectToAction(nameof(Index));
        }
        //[HttpPost]
        [Authorize(Roles = "Manager,Admin")]
        public async Task<IActionResult> CancelOrder(int id)
        {
            var order = await _unitOfWork.Orders.GetByIdAsync(id);
            if (order == null) return NotFound();

            order.Status = "Cancelled";
            _unitOfWork.Orders.Update(order);
            await _unitOfWork.CompleteAsync();

            //return RedirectToAction(nameof(Details), new { id });
            return RedirectToAction(nameof(Index));
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
