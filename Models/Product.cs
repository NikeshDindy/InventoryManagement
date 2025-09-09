namespace InventoryManagement.Models
{
    public class Product
    {
        public int ProductId { get; set; }
        public string SKU { get; set; }   // Stock Keeping Unit (unique code)
        public string Name { get; set; }
        public string? Description { get; set; }
        public decimal UnitPrice { get; set; }
        public int StockQuantity { get; set; }
        public int LowStockThreshold { get; set; }

        public int CategoryId { get; set; }
        public Category Category { get; set; }

        public int SupplierId { get; set; }
        public Supplier Supplier { get; set; }

        public ICollection<OrderDetail> OrderDetails { get; set; }
        public ICollection<InventoryTransaction> Transactions { get; set; }
        public int Id { get; internal set; }
    }
}
