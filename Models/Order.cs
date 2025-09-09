namespace InventoryManagement.Models
{
    public class Order
    {
        public int OrderId { get; set; }
        public string OrderNumber { get; set; }
        public string OrderType { get; set; }  // "Purchase" or "Sales"
        public string Status { get; set; }     // Pending, Fulfilled, Cancelled
        public DateTime CreatedAt { get; set; }
        public decimal TotalAmount { get; set; }

        public string CreatedByUserId { get; set; }
        public ApplicationUser CreatedBy { get; set; }

        public int? SupplierId { get; set; }   // Only for Purchase Orders
        public Supplier? Supplier { get; set; }

        public ICollection<OrderDetail> OrderDetails { get; set; }
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
