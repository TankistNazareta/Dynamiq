using Dynamiq.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Dynamiq.Infrastructure.Persistence.Configurations
{
    public class SubscriptionConfiguration : IEntityTypeConfiguration<Subscription>
    {
        public void Configure(EntityTypeBuilder<Subscription> builder)
        {
            builder.HasKey(s => s.Id);

            builder.Property(s => s.StartDate)
                   .IsRequired();

            builder.Property(s => s.EndDate)
                   .IsRequired();

            builder.Property(s => s.UserId)
                   .IsRequired();

            builder.Property(s => s.ProductId)
                   .IsRequired();

            builder.Property(s => s.PaymentHistoryId)
                    .IsRequired();

            builder.HasOne<Product>()
                   .WithMany(p => p.Subscriptions)
                   .HasForeignKey(s => s.ProductId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne<User>()
                   .WithMany(u => u.Subscriptions)
                   .HasForeignKey(s => s.UserId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne<PaymentHistory>()
                   .WithMany()
                   .HasForeignKey(s => s.PaymentHistoryId)
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
