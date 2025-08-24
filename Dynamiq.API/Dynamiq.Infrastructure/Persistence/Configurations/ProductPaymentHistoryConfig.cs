using Dynamiq.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Dynamiq.Infrastructure.Persistence.Configurations
{
    public class ProductPaymentHistoryConfig : IEntityTypeConfiguration<ProductPaymentHistory>
    {
        public void Configure(EntityTypeBuilder<ProductPaymentHistory> builder)
        {
            builder.HasKey(p => p.Id);
            builder.Property(rt => rt.Id)
                   .ValueGeneratedOnAdd()
                   .HasDefaultValueSql("NEWID()");

            builder.Property(p => p.Quantity)
                   .IsRequired();

            builder.Property(p => p.ProductId)
                   .IsRequired();

            builder.Property(p => p.PaymentHistoryId)
                   .IsRequired();
        }
    }
}
