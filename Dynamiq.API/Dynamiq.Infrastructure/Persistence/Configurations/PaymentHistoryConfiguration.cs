using Dynamiq.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Dynamiq.Infrastructure.Persistence.Configurations
{
    public class PaymentHistoryConfiguration : IEntityTypeConfiguration<PaymentHistory>
    {
        public void Configure(EntityTypeBuilder<PaymentHistory> builder)
        {
            builder.HasKey(p => p.Id);

            builder.Property(p => p.StripePaymentId)
                   .IsRequired()
                   .HasMaxLength(100);

            builder.Property(p => p.Amount)
                   .HasColumnType("decimal(18,2)")
                   .IsRequired();

            builder.Property(p => p.Interval)
                   .IsRequired();

            builder.Property(p => p.CreatedAt)
                   .IsRequired();

            builder.Property(p => p.UserId)
                   .IsRequired();

            builder.Property(p => p.ProductId)
                   .IsRequired();

            builder.HasOne<User>()
                   .WithMany(u => u.PaymentHistories)
                   .HasForeignKey(p => p.UserId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne<Product>()
                   .WithMany(prod => prod.PaymentHistories)
                   .HasForeignKey(p => p.ProductId)
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
