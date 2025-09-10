using System.ComponentModel.DataAnnotations;

namespace InventoryManagement.Models
{
    public class Category
    {
        [Key]
        public int CategoryId { get; set; }

        [Required(ErrorMessage = "Category name is required.")]
        [StringLength(100, ErrorMessage = "Category name cannot exceed 100 characters.")]
        public string Name { get; set; }

        [StringLength(250, ErrorMessage = "Description cannot exceed 250 characters.")]
        public string? Description { get; set; }

        public ICollection<Product> Products { get; set; }
    }
}
