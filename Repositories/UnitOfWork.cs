using InventoryManagement.Data;
using InventoryManagement.Models;
using Microsoft.EntityFrameworkCore;

namespace InventoryManagement.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;
        public IRepository<Product> Products { get; }
        public IRepository<Category> Categories { get; }
        public IRepository<Supplier> Suppliers { get; }
        public IRepository<Order> Orders { get; }
        public IRepository<OrderDetail> OrderDetails { get; }
        public IRepository<InventoryTransaction> Transactions { get; }

        public UnitOfWork(ApplicationDbContext context)
        {
            _context = context;
            Products = new Repository<Product>(_context);
            Categories = new Repository<Category>(_context);
            Suppliers = new Repository<Supplier>(_context);
            Orders = new Repository<Order>(_context);
            OrderDetails = new Repository<OrderDetail>(_context);
            Transactions = new Repository<InventoryTransaction>(_context);
        }

        public async Task<int> CompleteAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
