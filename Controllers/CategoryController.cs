using InventoryManagement.Models;
using InventoryManagement.Repositories;
using InventoryManagement.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace InventoryManagement.Controllers
{
    public class CategoryController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<CategoryController> _logger;

        public CategoryController(IUnitOfWork unitOfWork, ILogger<CategoryController> logger)
        {
            this._unitOfWork = unitOfWork;
            this._logger = logger;
        }

        //GET: /Category
        public async Task<IActionResult> Index()
        {
            var categories = await  _unitOfWork.Categories.GetAllAsync();
            return View(categories);
        }

        //GET: /Category/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return BadRequest();

            var category = await _unitOfWork.Categories.GetByIdAsync(id.Value);

            if(category == null) return NotFound();

            return View(category);
        }

        //GET: /Category/Create
        [Authorize(Roles = "Admin, Manager")]
        public IActionResult Create()
        {
            return View();
        }

        //POST: /Category/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CategoryCreateDto dto)
        {
            if (!ModelState.IsValid)
            {
                return View(dto); // Pass dto back to re-render form with validation errors
            }

            var category = new Category
            {
                Name = dto.Name,
                Description = dto.Description
            };

            await _unitOfWork.Categories.AddAsync(category);
            await _unitOfWork.CompleteAsync();

            TempData["Success"] = "Category created successfully.";
            return RedirectToAction(nameof(Index));
        }

        // GET: /Category/Edit/5
        [Authorize(Roles = "Admin, Manager")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return BadRequest();

            var category = await _unitOfWork.Categories.GetByIdAsync(id.Value);
            if (category == null) return NotFound();

            return View(category);
        }

        // POST: /Category/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, CategoryEditDto dto)
        {
            if (id != dto.CategoryId) return BadRequest();

            if (!ModelState.IsValid) return View(dto);

            try
            {
                var category = await _unitOfWork.Categories.GetByIdAsync(dto.CategoryId);
                if (category == null) return NotFound();

                // Map DTO → Entity
                category.Name = dto.Name;
                category.Description = dto.Description;

                _unitOfWork.Categories.Update(category);
                await _unitOfWork.CompleteAsync();

                TempData["Success"] = "Category updated successfully.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating category id {CategoryId}", dto.CategoryId);
                ModelState.AddModelError("", "Unable to update category. Try again.");
                return View(dto);
            }
        }

        // GET: /Category/Delete/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return BadRequest();

            var category = await _unitOfWork.Categories.GetByIdAsync(id.Value);
            if (category == null) return NotFound();

            return View(category);
        }

        // POST: /Category/DeleteConfirmed/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var category = await _unitOfWork.Categories.GetByIdAsync(id);
            if (category == null) return NotFound();

            try
            {
                _logger.LogError("---------------------inside confirm delete");
                _unitOfWork.Categories.Remove(category);
                await _unitOfWork.CompleteAsync();

                TempData["Success"] = "Category deleted successfully.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "-------------------Error deleting category id {CategoryId}", id);
                TempData["Error"] = "Unable to delete category. It may be referenced by products.";
                return RedirectToAction(nameof(Delete), new { id });
            }
        }

    }

    
}
