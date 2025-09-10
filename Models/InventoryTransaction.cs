using System.ComponentModel.DataAnnotations;

namespace InventoryManagement.Models
{
    public class InventoryTransaction
    {
        [Key] // Primary Key
        public int TransactionId { get; set; }

        // Foreign Key to Product
        [Required]
        public int ProductId { get; set; }

        [Required]
        public Product Product { get; set; } = null!;

        // Quantity of the transaction
        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be at least 1.")]
        public int Quantity { get; set; }

        // Type of transaction: "IN" or "OUT"
        [Required]
        [StringLength(10, ErrorMessage = "Transaction type cannot exceed 10 characters.")]
        public string TransactionType { get; set; } = string.Empty;

        // Timestamp defaults to now
        [Required]
        public DateTime Timestamp { get; set; } = DateTime.Now;

        // Optional link to an order
        public int? OrderId { get; set; }
        public Order? Order { get; set; }

        // User who performed the transaction
        public string? PerformedByUserId { get; set; }
        public ApplicationUser? PerformedBy { get; set; }

        // Optional notes
        [StringLength(500, ErrorMessage = "Notes cannot exceed 500 characters.")]
        public string? Notes { get; set; }
    }
}
