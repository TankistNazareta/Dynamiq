using Dynamiq.Domain.Aggregates;
using Dynamiq.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Dynamiq.Infrastructure.Persistence.Configurations
{
    public class PaymentHistoryConfiguration : IEntityTypeConfiguration<PaymentHistory>
    {
        public void Configure(EntityTypeBuilder<PaymentHistory> builder)
        {
            builder.HasKey(ph => ph.Id);

            builder.Property(ph => ph.StripeTransactionId)
                .IsRequired()
                .HasMaxLength(200)
                .HasColumnName("StripeTransactionId");

            builder.Property(ph => ph.Amount)
                .IsRequired()
                .HasColumnType("decimal(18,2)");

            builder.Property(ph => ph.CreatedAt)
                .IsRequired();

            builder.Property(ph => ph.UserId)
                .IsRequired();

            builder.HasMany(ph => ph.Products)
                   .WithOne()
                   .HasForeignKey(pph => pph.PaymentHistoryId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(ph => ph.Subscription)
                   .WithOne()
                   .HasForeignKey<SubscriptionHistory>(sh => sh.PaymentHistoryId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.ToTable("PaymentHistories");
        }
    }
}
