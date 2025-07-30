using Dynamiq.Domain.Aggregates;
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
                    .HasColumnName("Amount")
                    .HasColumnType("decimal(18,2)");

            builder.Property(p => p.Interval)
                   .IsRequired();

            builder.Property(p => p.CreatedAt)
                   .IsRequired();

            builder.Property(p => p.UserId)
                   .IsRequired();

            builder.HasOne<User>()
                   .WithMany(u => u.PaymentHistories)
                   .HasForeignKey(p => p.UserId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.Navigation(p => p.Products).Metadata.SetField("_products");
            builder.Navigation(p => p.Products).UsePropertyAccessMode(PropertyAccessMode.Field);
        }
    }
}
