using System.ComponentModel.DataAnnotations;

namespace InventoryManagement.ViewModels
{
    public class PurchaseOrderDto
    {
        [Required(ErrorMessage = "Order Number is required")]
        public string OrderNumber { get; set; }

        [Required(ErrorMessage = "Supplier is required")]
        public int SupplierId { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Total must be positive")]
        public decimal TotalAmount { get; set; }
    }
}
