using InventoryManagement.Models;
using InventoryManagement.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace InventoryManagement.Controllers
{
    [Authorize(Roles = "Admin,Manager,Staff")]
    public class SupplierController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public SupplierController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        // =======================
        // VIEW SUPPLIERS (All roles)
        // =======================
        [Authorize(Roles = "Admin,Manager,Staff")]
        public async Task<IActionResult> Index()
        {
            var suppliers = await _unitOfWork.Suppliers.GetAllAsync();
            return View(suppliers);
        }

        [Authorize(Roles = "Admin,Manager,Staff")]
        public async Task<IActionResult> Details(int id)
        {
            var supplier = await _unitOfWork.Suppliers.GetByIdAsync(id);
            if (supplier == null)
            {
                return NotFound();
            }
            return View(supplier);
        }

        // =======================
        // CREATE SUPPLIER (Admin, Manager)
        // =======================
        [Authorize(Roles = "Admin,Manager")]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> Create([Bind("SupplierId,Name,ContactInfo")] Supplier supplier)
        {
            if (ModelState.IsValid)
            {
                await _unitOfWork.Suppliers.AddAsync(supplier);
                await _unitOfWork.CompleteAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(supplier);
        }

        // =======================
        // EDIT SUPPLIER (Admin, Manager)
        // =======================
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> Edit(int id)
        {
            var supplier = await _unitOfWork.Suppliers.GetByIdAsync(id);
            if (supplier == null)
            {
                return NotFound();
            }
            return View(supplier);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> Edit(int id, [Bind("SupplierId,Name,ContactInfo")] Supplier supplier)
        {
            if (id != supplier.SupplierId)
                return NotFound();

            if (ModelState.IsValid)
            {
                _unitOfWork.Suppliers.Update(supplier);
                await _unitOfWork.CompleteAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(supplier);
        }

        // =======================
        // DELETE SUPPLIER (Admin only)
        // =======================
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var supplier = await _unitOfWork.Suppliers.GetByIdAsync(id);
            if (supplier == null)
            {
                return NotFound();
            }
            return View(supplier);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var supplier = await _unitOfWork.Suppliers.GetByIdAsync(id);
            if (supplier != null)
            {
                _unitOfWork.Suppliers.Remove(supplier);
                await _unitOfWork.CompleteAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
