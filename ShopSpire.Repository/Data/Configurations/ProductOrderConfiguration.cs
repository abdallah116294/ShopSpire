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
    public class ProductOrderConfiguration : IEntityTypeConfiguration<ProductOrder>
    {
        public void Configure(EntityTypeBuilder<ProductOrder> builder)
        {
            builder.HasKey(po => po.Id);

            builder.HasOne(po => po.Order)
                   .WithMany(o => o.ProductOrders)
                   .HasForeignKey(po => po.OrderId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(po => po.Product)
                   .WithMany()
                   .HasForeignKey(po => po.ProductId)
                   .OnDelete(DeleteBehavior.Restrict); // Don't delete products when order is deleted

            builder.Property(po => po.UnitPrice)
                   .HasColumnType("decimal(18,2)")
                   .IsRequired();

            builder.Property(po => po.Quantity)
                   .IsRequired();
        }
    }
}
