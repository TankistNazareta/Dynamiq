using Dynamiq.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Dynamiq.Infrastructure.Persistence.Configurations
{
    public class ProductPaymentHistoryConfiguration : IEntityTypeConfiguration<ProductPaymentHistory>
    {
        public void Configure(EntityTypeBuilder<ProductPaymentHistory> builder)
        {
            builder.HasKey(pph => pph.Id);

            builder.Property(pph => pph.ProductId)
                .IsRequired();

            builder.Property(pph => pph.PaymentHistoryId)
                .IsRequired();

            builder.Property(pph => pph.Quantity)
                .IsRequired();

            builder.ToTable("ProductPaymentHistories");
        }
    }
}
