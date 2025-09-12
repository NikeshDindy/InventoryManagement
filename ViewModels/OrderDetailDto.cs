using InventoryManagement.Models;

namespace InventoryManagement.ViewModels
{
    public class OrderDetailDto
    {
        public int OrderId { get; set; }
        public string OrderNumber { get; set; }
        public string OrderType { get; set; }
        public string Status { get; set; }
        public decimal TotalAmount { get; set; }
        public string CreatedByUserId { get; set; }
        public string CreatedByName { get; set; } // Optional, if you want to show user name
        public string SupplierName { get; set; }   // Optional
        public DateTime CreatedAt { get; set; }

        public ICollection<OrderDetail> OrderDetails { get; set; }
        public ICollection<InventoryTransaction> InventoryTransactions { get; set; }
    }
}
