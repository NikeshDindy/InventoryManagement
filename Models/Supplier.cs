using System.ComponentModel.DataAnnotations;

namespace InventoryManagement.Models
{
    public class Supplier
    {
        [Key] // Primary Key
        public int SupplierId { get; set; }

        [Required(ErrorMessage = "Supplier name is required.")]
        [StringLength(100, ErrorMessage = "Name cannot exceed 100 characters.")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Contact information is required.")]
        [StringLength(100, ErrorMessage = "Contact info cannot exceed 100 characters.")]
        public string ContactInfo { get; set; } = string.Empty;

        [StringLength(200, ErrorMessage = "Address cannot exceed 200 characters.")]
        public string? Address { get; set; }

        // Navigation properties
        public ICollection<Product> Products { get; set; } = new HashSet<Product>();
        public ICollection<Order> Orders { get; set; } = new HashSet<Order>();
    }
}
