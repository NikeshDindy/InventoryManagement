using InventoryManagement.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace InventoryManagement.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) { }

        // DbSets
        public DbSet<Category> Categories { get; set; }
        public DbSet<Supplier> Suppliers { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderDetail> OrderDetails { get; set; }
        public DbSet<InventoryTransaction> InventoryTransactions { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // ---------------- Product ----------------
            builder.Entity<Product>()
                   .HasOne(p => p.Category)
                   .WithMany(c => c.Products)
                   .HasForeignKey(p => p.CategoryId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Product>()
                   .HasOne(p => p.Supplier)
                   .WithMany(s => s.Products)
                   .HasForeignKey(p => p.SupplierId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Product>()
                   .Property(p => p.UnitPrice)
                   .HasPrecision(18, 2);

            // ---------------- Order ----------------
            builder.Entity<Order>()
                   .HasOne(o => o.CreatedBy)
                   .WithMany()
                   .HasForeignKey(o => o.CreatedByUserId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Order>()
                   .HasOne(o => o.Supplier)
                   .WithMany(s => s.Orders)
                   .HasForeignKey(o => o.SupplierId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Order>()
                   .Property(o => o.TotalAmount)
                   .HasPrecision(18, 2);

            // ---------------- OrderDetail ----------------
            builder.Entity<OrderDetail>()
                   .HasOne(od => od.Order)
                   .WithMany(o => o.OrderDetails)
                   .HasForeignKey(od => od.OrderId);

            builder.Entity<OrderDetail>()
                   .HasOne(od => od.Product)
                   .WithMany(p => p.OrderDetails)
                   .HasForeignKey(od => od.ProductId);

            builder.Entity<OrderDetail>()
                   .Property(od => od.UnitPrice)
                   .HasPrecision(18, 2);

            builder.Entity<OrderDetail>()
                   .Property(od => od.LineTotal)
                   .HasPrecision(18, 2);

            // ---------------- InventoryTransaction ----------------
            builder.Entity<InventoryTransaction>()
                   .HasKey(t => t.TransactionId);

            builder.Entity<InventoryTransaction>()
                   .HasOne(t => t.Product)
                   .WithMany(p => p.InventoryTransactions)
                   .HasForeignKey(t => t.ProductId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<InventoryTransaction>()
                   .HasOne(t => t.Order)
                   .WithMany(o => o.InventoryTransactions)
                   .HasForeignKey(t => t.OrderId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<InventoryTransaction>()
                   .HasOne(t => t.PerformedBy)
                   .WithMany()
                   .HasForeignKey(t => t.PerformedByUserId)
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }
}