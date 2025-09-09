namespace InventoryManagement.Models
{
    public class InventoryTransaction
    {
        public int TransactionId { get; set; }
        public int ProductId { get; set; }
        public Product Product { get; set; }

        public int Quantity { get; set; }
        public string TransactionType { get; set; }  // "IN" or "OUT"
        public DateTime Timestamp { get; set; }

        public int? OrderId { get; set; } // Optional: link to order
        public Order? Order { get; set; }

        public string PerformedByUserId { get; set; }
        public ApplicationUser PerformedBy { get; set; }

        public string? Notes { get; set; }
    }
}
