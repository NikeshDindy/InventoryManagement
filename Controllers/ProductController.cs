using InventoryManagement.Models;
using InventoryManagement.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Threading.Tasks;

[Authorize(Roles = "Admin,Manager,Staff")]
public class ProductController : Controller
{
    private readonly IUnitOfWork _unitOfWork;

    public ProductController(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    // GET: Product
    public async Task<IActionResult> Index()
    {
        var products = await _unitOfWork.Products.GetAllAsync();
        return View(products);
    }

    // GET: Product/Details/5
    public async Task<IActionResult> Details(int id)
    {
        var product = await _unitOfWork.Products.GetByIdAsync(id);
        if (product == null) return NotFound();
        return View(product);
    }

    // GET: Product/Create
    [Authorize(Roles = "Admin,Manager")]
    public async Task<IActionResult> Create()
    {
        ViewBag.Categories = new SelectList(await _unitOfWork.Categories.GetAllAsync(), "CategoryId", "Name");
        ViewBag.Suppliers = new SelectList(await _unitOfWork.Suppliers.GetAllAsync(), "SupplierId", "Name");
        return View();
    }

    // POST: Product/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<IActionResult> Create([Bind("ProductId,SKU,Name,Description,UnitPrice,StockQuantity,LowStockThreshold,CategoryId,SupplierId")] Product product)
    {
        if (ModelState.IsValid)
        {
            await _unitOfWork.Products.AddAsync(product);
            await _unitOfWork.CompleteAsync();
            return RedirectToAction(nameof(Index));
        }
        ViewBag.Categories = new SelectList(await _unitOfWork.Categories.GetAllAsync(), "CategoryId", "Name", product.CategoryId);
        ViewBag.Suppliers = new SelectList(await _unitOfWork.Suppliers.GetAllAsync(), "SupplierId", "Name", product.SupplierId);
        return View(product);
    }

    // GET: Product/Edit/5
    [Authorize(Roles = "Admin,Manager")]
    public async Task<IActionResult> Edit(int id)
    {
        var product = await _unitOfWork.Products.GetByIdAsync(id);
        if (product == null) return NotFound();
        ViewBag.Categories = new SelectList(await _unitOfWork.Categories.GetAllAsync(), "CategoryId", "Name", product.CategoryId);
        ViewBag.Suppliers = new SelectList(await _unitOfWork.Suppliers.GetAllAsync(), "SupplierId", "Name", product.SupplierId);
        return View(product);
    }

    // POST: Product/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<IActionResult> Edit(int id, [Bind("ProductId,SKU,Name,Description,UnitPrice,StockQuantity,LowStockThreshold,CategoryId,SupplierId")] Product product)
    {
        if (id != product.ProductId) return BadRequest();

        if (ModelState.IsValid)
        {
            _unitOfWork.Products.Update(product);
            await _unitOfWork.CompleteAsync();
            return RedirectToAction(nameof(Index));
        }
        ViewBag.Categories = new SelectList(await _unitOfWork.Categories.GetAllAsync(), "CategoryId", "Name", product.CategoryId);
        ViewBag.Suppliers = new SelectList(await _unitOfWork.Suppliers.GetAllAsync(), "SupplierId", "Name", product.SupplierId);
        return View(product);
    }

    // GET: Product/Delete/5
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        var product = await _unitOfWork.Products.GetByIdAsync(id);
        if (product == null) return NotFound();
        return View(product);
    }

    // POST: Product/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var product = await _unitOfWork.Products.GetByIdAsync(id);
        if (product == null) return NotFound();
        _unitOfWork.Products.Remove(product);
        await _unitOfWork.CompleteAsync();
        return RedirectToAction(nameof(Index));
    }
}
