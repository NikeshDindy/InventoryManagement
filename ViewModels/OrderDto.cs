namespace InventoryManagement.ViewModels
{
    public class OrderDto
    {
        public int OrderId { get; set; }
        public string OrderNumber { get; set; } = string.Empty;
        public string OrderType { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public string? SupplierName { get; set; }

        // Add this to fix the error
        public List<OrderDetailDto>? OrderDetails { get; set; }
    }
}
