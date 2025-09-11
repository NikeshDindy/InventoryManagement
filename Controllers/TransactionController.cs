using InventoryManagement.Models;
using InventoryManagement.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Security.Claims;

namespace InventoryManagement.Controllers
{
    public class TransactionController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<TransactionController> _logger;

        public TransactionController(IUnitOfWork unitOfWork, ILogger<TransactionController> logger)
        {
            this._unitOfWork = unitOfWork;
            this._logger = logger;
        }

        //GET: /Transaction
        public async Task<IActionResult> Index()
        {
            var transactions = await _unitOfWork.Transactions.GetAllAsync(t => t.Product, t => t.PerformedBy);

            return View(transactions);
        }

        // GET: /InventoryTransaction/RecordPurchase
        public async Task<IActionResult> RecordPurchase()
        {
            ViewBag.Products = new SelectList(await _unitOfWork.Products.GetAllAsync(), "ProductId", "Name");
            return View();
        }

        // POST: /InventoryTransaction/RecordPurchase
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RecordPurchase(int productId, int quantity, string? notes, int? orderId)
        {
            if (quantity <= 0)
            {
                ModelState.AddModelError("", "Quantity must be greater than zero.");
                ViewBag.Products = new SelectList(await _unitOfWork.Products.GetAllAsync(), "ProductId", "Name");
                return View();
            }

            var product = await _unitOfWork.Products.GetByIdAsync(productId);
            if (product == null) return NotFound();

            // Increase stock
            product.StockQuantity += quantity;

            var transaction = new InventoryTransaction
            {
                ProductId = productId,
                Quantity = quantity,
                TransactionType = "IN",
                Timestamp = DateTime.Now,
                Notes = notes,
                OrderId = orderId,
                PerformedByUserId = User.FindFirstValue(ClaimTypes.NameIdentifier)
                // in real apps, use logged-in user Id
            };

            await _unitOfWork.Transactions.AddAsync(transaction);
            _unitOfWork.Products.Update(product);
            await _unitOfWork.CompleteAsync();

            TempData["Success"] = "Purchase recorded successfully.";
            return RedirectToAction(nameof(Index));
        }

        // GET: /InventoryTransaction/RecordSale
        public async Task<IActionResult> RecordSale()
        {
            ViewBag.Products = new SelectList(await _unitOfWork.Products.GetAllAsync(), "ProductId", "Name");
            return View();
        }

        // POST: /InventoryTransaction/RecordSale
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RecordSale(int productId, int quantity, string? notes, int? orderId)
        {
            if (quantity <= 0)
            {
                ModelState.AddModelError("", "Quantity must be greater than zero.");
                ViewBag.Products = new SelectList(await _unitOfWork.Products.GetAllAsync(), "ProductId", "Name");
                return View();
            }

            var product = await _unitOfWork.Products.GetByIdAsync(productId);
            if (product == null) return NotFound();

            if (product.StockQuantity < quantity)
            {
                ModelState.AddModelError("", "Not enough stock available.");
                ViewBag.Products = new SelectList(await _unitOfWork.Products.GetAllAsync(), "ProductId", "Name");
                return View();
            }

            // Decrease stock
            product.StockQuantity -= quantity;

            var transaction = new InventoryTransaction
            {
                ProductId = productId,
                Quantity = quantity,
                TransactionType = "OUT",
                Timestamp = DateTime.Now,
                Notes = notes,
                OrderId = orderId,
                PerformedByUserId = User.FindFirstValue(ClaimTypes.NameIdentifier)

            };

            await _unitOfWork.Transactions.AddAsync(transaction);
            _unitOfWork.Products.Update(product);
            await _unitOfWork.CompleteAsync();

            TempData["Success"] = "Sale recorded successfully.";
            return RedirectToAction(nameof(Index));
        }

        // GET: /Transaction/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var transaction = await _unitOfWork.Transactions.GetByIdAsync(id);
            if (transaction == null) return NotFound();

            return View(transaction);
        }

        // POST: /Transaction/DeleteConfirmed/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var transaction = await _unitOfWork.Transactions.GetByIdAsync(id);
            if (transaction == null) return NotFound();

            var product = await _unitOfWork.Products.GetByIdAsync(transaction.ProductId);
            if (product != null)
            {
                // Revert stock
                if (transaction.TransactionType == "IN")
                    product.StockQuantity -= transaction.Quantity; // undo purchase
                else if (transaction.TransactionType == "OUT")
                    product.StockQuantity += transaction.Quantity; // undo sale

                _unitOfWork.Products.Update(product);
            }

            _unitOfWork.Transactions.Remove(transaction);
            await _unitOfWork.CompleteAsync();

            TempData["Success"] = "Transaction deleted successfully (stock reverted).";
            return RedirectToAction(nameof(Index));
        }

    }
}
