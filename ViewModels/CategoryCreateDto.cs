using System.ComponentModel.DataAnnotations;

namespace InventoryManagement.ViewModels
{
    public class CategoryCreateDto
    {
        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        [StringLength(500)]
        public string? Description { get; set; }
    }
}
