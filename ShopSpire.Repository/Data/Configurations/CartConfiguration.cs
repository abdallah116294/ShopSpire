using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ShopSpireCore.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShopSpire.Repository.Data.Configurations
{
    public class CartConfiguration : IEntityTypeConfiguration<Cart>
    {
        public void Configure(EntityTypeBuilder<Cart> builder)
        {
            builder.ToTable("Carts");
            builder.HasKey(c => c.Id);
            builder.HasOne(c => c.User)
          .WithMany()
          .HasForeignKey(c => c.UserId)
          .OnDelete(DeleteBehavior.Cascade);
            builder.HasOne(c => c.Product)
          .WithMany()
          .HasForeignKey(c => c.ProductId)
          .OnDelete(DeleteBehavior.Cascade);
            builder.Property(c => c.Quantity)
           .IsRequired();
            // Ensure unique combination of UserId and ProductId
            builder.HasIndex(c => new { c.UserId, c.ProductId })
                   .IsUnique();
        }
    }
}
