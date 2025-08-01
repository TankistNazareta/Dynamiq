using Dynamiq.Domain.Aggregates;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Dynamiq.Infrastructure.Persistence.Configurations
{
    public class ProductConfiguration : IEntityTypeConfiguration<Product>
    {
        public void Configure(EntityTypeBuilder<Product> builder)
        {
            builder.HasKey(p => p.Id);
            builder.Property(rt => rt.Id)
                   .ValueGeneratedOnAdd()
                   .HasDefaultValueSql("NEWID()");

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

            builder.Property(p => p.Interval)
                   .IsRequired();

            builder.Property(p => p.CategoryId)
                    .IsRequired();

            builder.HasMany(p => p.ProductPaymentHistories)
                   .WithOne()
                   .HasForeignKey(ph => ph.ProductId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne<Category>()
                .WithMany()
                .HasForeignKey(p => p.CategoryId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
