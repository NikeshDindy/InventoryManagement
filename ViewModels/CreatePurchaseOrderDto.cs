namespace InventoryManagement.ViewModels
{
    public class CreatePurchaseOrderDto
    {
        public int SupplierId { get; set; }

        // Optional: user can enter it, or system will auto-generate
        public string? OrderNumber { get; set; }
    }
}
