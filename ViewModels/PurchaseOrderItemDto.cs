using System.ComponentModel.DataAnnotations;

namespace InventoryManagement.ViewModels
{
    public class PurchaseOrderItemDto
    {
        [Required]
        public int ProductId { get; set; }

        public string? SKU { get; set; }             // nullable, display only
        public string? ProductName { get; set; }     // nullable, display only
        public string? CategoryName { get; set; }    // nullable, display only

        [Range(0, int.MaxValue, ErrorMessage = "Quantity must be at least 0")]
        public int Quantity { get; set; }

        public decimal UnitPrice { get; set; }

        public decimal LineTotal => Quantity * UnitPrice;
    }
}
