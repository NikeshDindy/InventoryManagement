using Microsoft.AspNetCore.Mvc;

namespace InventoryManagement.Data
{
    public class ProductController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
