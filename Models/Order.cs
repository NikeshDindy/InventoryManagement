using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InventoryManagement.Models
{
    public class Order
    {
        [Key]
        public int OrderId { get; set; }

        [Required, StringLength(20)]
        public string OrderNumber { get; set; } = string.Empty;

        [Required, StringLength(20)]
        public string OrderType { get; set; } = string.Empty;
        // "Purchase" or "Sales"

        [Required, StringLength(20)]
        public string Status { get; set; } = "Pending";
        // Pending, Fulfilled, Cancelled

        [DataType(DataType.DateTime)]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Column(TypeName = "decimal(18,2)")]
        [Range(0, double.MaxValue, ErrorMessage = "Total must be positive")]
        public decimal TotalAmount { get; set; }

        // User who created the order
        [Required]
        public string CreatedByUserId { get; set; } = string.Empty;

        [ForeignKey(nameof(CreatedByUserId))]
        public ApplicationUser CreatedBy { get; set; }

        // Supplier only applies to Purchase orders
        public int? SupplierId { get; set; }

        [ForeignKey(nameof(SupplierId))]
        public Supplier? Supplier { get; set; }

        public ICollection<OrderDetail> OrderDetails { get; set; }

        public ICollection<InventoryTransaction> InventoryTransactions { get; set; }
        public ICollection<InventoryTransaction> InventoryTransactions { get; set; }
    }
}



// functionality of models
//User
//↳ creates Orders
//↳ performs Transactions

//Category
//↳ has many Products

//Supplier
//↳ has many Products
//↳ linked to Purchase Orders

//Product
//↳ belongs to Category + Supplier
//↳ has many OrderDetails
//↳ has many Transactions

//Order
//↳ has many OrderDetails
//↳ may belong to Supplier (if Purchase)

//OrderDetail
//↔ Many-to-Many between Products and Orders

//Transaction
//↳ belongs to Product
//↳ linked to User
//↳ optional link to Order
