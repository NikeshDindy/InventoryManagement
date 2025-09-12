using System.ComponentModel.DataAnnotations;

namespace InventoryManagement.ViewModels
{
    public class PurchaseOrderDto
    {
        [Required(ErrorMessage = "Order Number is required")]
        public string OrderNumber { get; set; } = string.Empty;

        [Required(ErrorMessage = "Supplier is required")]
        public int SupplierId { get; set; }

        public List<PurchaseOrderItemDto> Items { get; set; } = new List<PurchaseOrderItemDto>();
    }
}
