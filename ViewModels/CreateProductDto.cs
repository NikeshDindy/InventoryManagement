namespace InventoryManagement.ViewModels { 
public class CreateProductDto
{
    public string SKU { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public decimal UnitPrice { get; set; }
    public int StockQuantity { get; set; }
    public int LowStockThreshold { get; set; }
    public int CategoryId { get; set; }
    public int SupplierId { get; set; }
}
}