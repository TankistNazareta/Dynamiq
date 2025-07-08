using Dynamiq.API.DAL.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Dynamiq.API.DAL.Context.Configurations
{
    public class ProductConfiguration : IEntityTypeConfiguration<Product>
    {
        public void Configure(EntityTypeBuilder<Product> builder)
        {
            builder.HasKey(p => p.Id);

            builder.Property(p => p.StripeProductId)
                   .IsRequired()
                   .HasMaxLength(100);

            builder.Property(p => p.StripePriceId)
                   .IsRequired()
                   .HasMaxLength(100);

            builder.Property(p => p.Name)
                   .IsRequired()
                   .HasMaxLength(200);

            builder.Property(p => p.Description)
                   .HasMaxLength(1000);

            builder.Property(p => p.Price)
                   .IsRequired();

            builder.HasMany(p => p.PaymentHistories)
                   .WithOne(ph => ph.Product)
                   .HasForeignKey(ph => ph.ProductId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(p => p.Subscriptions)
               .WithOne(s => s.Product)
               .HasForeignKey(s => s.ProductId)
               .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
