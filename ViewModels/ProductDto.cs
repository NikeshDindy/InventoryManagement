namespace InventoryManagement.ViewModels
{
    public class ProductDto
    {
        public int ProductId { get; set; }

        public string SKU { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public decimal UnitPrice { get; set; }

        public int StockQuantity { get; set; }

        public int LowStockThreshold { get; set; }

        public int CategoryId { get; set; }
        public string CategoryName { get; set; }   // Useful when displaying

        public int SupplierId { get; set; }
        public string SupplierName { get; set; }   // Useful when displaying
    }
}
