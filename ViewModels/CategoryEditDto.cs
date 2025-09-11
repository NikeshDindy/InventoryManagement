namespace InventoryManagement.ViewModels
{
    public class CategoryEditDto
    {
        public int CategoryId { get; set; }   // Needed to know which category to update
        public string Name { get; set; }
        public string Description { get; set; }
    }
}
