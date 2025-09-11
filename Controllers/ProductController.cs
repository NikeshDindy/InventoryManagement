using InventoryManagement.ViewModels;
using InventoryManagement.Models;
using InventoryManagement.Repositories;
using InventoryManagement.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Linq;
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
        var categories = await _unitOfWork.Categories.GetAllAsync();
        var suppliers = await _unitOfWork.Suppliers.GetAllAsync();

        var productDtos = products.Select(p => new ProductDto
        {
            ProductId = p.ProductId,
            SKU = p.SKU,
            Name = p.Name,
            Description = p.Description,
            UnitPrice = p.UnitPrice,
            StockQuantity = p.StockQuantity,
            LowStockThreshold = p.LowStockThreshold,
            CategoryId = p.CategoryId,
            CategoryName = categories.FirstOrDefault(c => c.CategoryId == p.CategoryId)?.Name,
            SupplierId = p.SupplierId,
            SupplierName = suppliers.FirstOrDefault(s => s.SupplierId == p.SupplierId)?.Name
        }).ToList();

        return View(productDtos);
    }

    // GET: Product/Details/5
    public async Task<IActionResult> Details(int id)
    {
        var product = await _unitOfWork.Products.GetByIdAsync(id);
        if (product == null) return NotFound();

        var category = await _unitOfWork.Categories.GetByIdAsync(product.CategoryId);
        var supplier = await _unitOfWork.Suppliers.GetByIdAsync(product.SupplierId);

        var dto = new ProductDto
        {
            ProductId = product.ProductId,
            SKU = product.SKU,
            Name = product.Name,
            Description = product.Description,
            UnitPrice = product.UnitPrice,
            StockQuantity = product.StockQuantity,
            LowStockThreshold = product.LowStockThreshold,
            CategoryId = product.CategoryId,
            CategoryName = category?.Name,
            SupplierId = product.SupplierId,
            SupplierName = supplier?.Name
        };

        return View(dto);
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
    public async Task<IActionResult> Create(CreateProductDto dto)
    {
        if (ModelState.IsValid)
        {
            var product = new Product
            {
                SKU = dto.SKU,
                Name = dto.Name,
                Description = dto.Description,
                UnitPrice = dto.UnitPrice,
                StockQuantity = dto.StockQuantity,
                LowStockThreshold = dto.LowStockThreshold,
                CategoryId = dto.CategoryId,
                SupplierId = dto.SupplierId
            };

            await _unitOfWork.Products.AddAsync(product);
            await _unitOfWork.CompleteAsync();
            return RedirectToAction(nameof(Index));
        }

        ViewBag.Categories = new SelectList(await _unitOfWork.Categories.GetAllAsync(), "CategoryId", "Name", dto.CategoryId);
        ViewBag.Suppliers = new SelectList(await _unitOfWork.Suppliers.GetAllAsync(), "SupplierId", "Name", dto.SupplierId);
        return View(dto);
    }

    // GET: Product/Edit/5
    [Authorize(Roles = "Admin,Manager")]
    public async Task<IActionResult> Edit(int id)
    {
        var product = await _unitOfWork.Products.GetByIdAsync(id);
        if (product == null) return NotFound();

        var dto = new UpdateProductDto
        {
            ProductId = product.ProductId,
            SKU = product.SKU,
            Name = product.Name,
            Description = product.Description,
            UnitPrice = product.UnitPrice,
            StockQuantity = product.StockQuantity,
            LowStockThreshold = product.LowStockThreshold,
            CategoryId = product.CategoryId,
            SupplierId = product.SupplierId
        };

        ViewBag.Categories = new SelectList(await _unitOfWork.Categories.GetAllAsync(), "CategoryId", "Name", dto.CategoryId);
        ViewBag.Suppliers = new SelectList(await _unitOfWork.Suppliers.GetAllAsync(), "SupplierId", "Name", dto.SupplierId);
        return View(dto);
    }

    // POST: Product/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<IActionResult> Edit(int id, UpdateProductDto dto)
    {
        if (id != dto.ProductId) return BadRequest();

        if (ModelState.IsValid)
        {
            var product = new Product
            {
                ProductId = dto.ProductId,
                SKU = dto.SKU,
                Name = dto.Name,
                Description = dto.Description,
                UnitPrice = dto.UnitPrice,
                StockQuantity = dto.StockQuantity,
                LowStockThreshold = dto.LowStockThreshold,
                CategoryId = dto.CategoryId,
                SupplierId = dto.SupplierId
            };

            _unitOfWork.Products.Update(product);
            await _unitOfWork.CompleteAsync();
            return RedirectToAction(nameof(Index));
        }

        ViewBag.Categories = new SelectList(await _unitOfWork.Categories.GetAllAsync(), "CategoryId", "Name", dto.CategoryId);
        ViewBag.Suppliers = new SelectList(await _unitOfWork.Suppliers.GetAllAsync(), "SupplierId", "Name", dto.SupplierId);
        return View(dto);
    }

    // GET: Product/Delete/5
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        var product = await _unitOfWork.Products.GetByIdAsync(id);
        if (product == null) return NotFound();

        var category = await _unitOfWork.Categories.GetByIdAsync(product.CategoryId);
        var supplier = await _unitOfWork.Suppliers.GetByIdAsync(product.SupplierId);

        var dto = new ProductDto
        {
            ProductId = product.ProductId,
            SKU = product.SKU,
            Name = product.Name,
            Description = product.Description,
            UnitPrice = product.UnitPrice,
            StockQuantity = product.StockQuantity,
            LowStockThreshold = product.LowStockThreshold,
            CategoryId = product.CategoryId,
            CategoryName = category?.Name,
            SupplierId = product.SupplierId,
            SupplierName = supplier?.Name
        };

        return View(dto);
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
