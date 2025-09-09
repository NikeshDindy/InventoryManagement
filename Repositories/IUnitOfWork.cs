using InventoryManagement.Models;

namespace InventoryManagement.Repositories
{
    public interface IUnitOfWork : IDisposable
    {
        IRepository<Product> Products { get; }
        IRepository<Category> Categories { get; }
        IRepository<Supplier> Suppliers { get; }
        IRepository<Order> Orders { get; }
        IRepository<OrderDetail> OrderDetails { get; }
        IRepository<InventoryTransaction> Transactions { get; }

        Task<int> CompleteAsync();
    }
}
