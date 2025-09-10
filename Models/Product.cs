using System.ComponentModel.DataAnnotations;

namespace InventoryManagement.Models
{
    public class Product
    {
        [Key] // Primary Key
        public int ProductId { get; set; }

        [Required(ErrorMessage = "SKU is required.")]
        [StringLength(50, ErrorMessage = "SKU cannot exceed 50 characters.")]
        public string SKU { get; set; } = string.Empty;

        [Required(ErrorMessage = "Product name is required.")]
        [StringLength(100, ErrorMessage = "Product name cannot exceed 100 characters.")]
        public string Name { get; set; } = string.Empty;

        [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters.")]
        public string? Description { get; set; }

        [Range(0.01, double.MaxValue, ErrorMessage = "Unit price must be greater than 0.")]
        public decimal UnitPrice { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Stock quantity cannot be negative.")]
        public int StockQuantity { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Low stock threshold cannot be negative.")]
        public int LowStockThreshold { get; set; }

        // Foreign Keys
        [Required]
        public int CategoryId { get; set; }
        public Category Category { get; set; } = null!;

        [Required]
        public int SupplierId { get; set; }
        public Supplier Supplier { get; set; } = null!;

        // Navigation properties
        public ICollection<OrderDetail> OrderDetails { get; set; } = new HashSet<OrderDetail>();
        public ICollection<InventoryTransaction> InventoryTransactions { get; set; } = new HashSet<InventoryTransaction>();
    }
}
