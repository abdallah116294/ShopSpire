using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ShopSpire.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShopSpire.DAL.Context
{
    public class ShopSpireContext : IdentityDbContext<User>
    {
        public ShopSpireContext(DbContextOptions options) : base(options)
        {

        }
        #region DbSets
        public DbSet<Category> Categories { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Cart> Carts { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<ProductOrder> ProductOrders { get; set; }
        public DbSet<Review> Reviews { get; set; }
        #endregion
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Product>()
        .Property(p => p.Price)
        .HasPrecision(18, 2);
            modelBuilder.Entity<Product>()
    .Property(p => p.Price)
    .HasColumnType("decimal(18,2)");
            modelBuilder.Entity<Cart>()
        .HasOne(c => c.Product)
        .WithMany()
        .HasForeignKey(c => c.ProductId)
        .OnDelete(DeleteBehavior.NoAction);
            modelBuilder.Entity<Review>()
    .HasOne(r => r.Product)
    .WithMany()
    .HasForeignKey(r => r.ProductId)
    .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<ProductOrder>()
                .HasOne(po => po.Product)
                .WithMany()
                .HasForeignKey(po => po.ProductId)
                .OnDelete(DeleteBehavior.NoAction);
            base.OnModelCreating(modelBuilder);
        }
    }
}
