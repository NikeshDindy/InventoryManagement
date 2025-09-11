namespace InventoryManagement.ViewModels
{
    public class OrderDto
    {
        public int OrderId { get; set; }
        public string OrderType { get; set; }
        public string Status { get; set; }
        public decimal TotalAmount { get; set; }
        public DateTime CreatedAt { get; set; }

        // Related info
        public string SupplierName { get; set; }
        public string CustomerName { get; set; }
    }
}
