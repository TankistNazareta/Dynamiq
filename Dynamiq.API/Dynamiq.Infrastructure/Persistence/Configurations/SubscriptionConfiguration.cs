using Dynamiq.Domain.Aggregates;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Dynamiq.Infrastructure.Persistence.Configurations
{
    public class SubscriptionConfiguration : IEntityTypeConfiguration<Subscription>
    {
        public void Configure(EntityTypeBuilder<Subscription> builder)
        {
            builder.HasKey(s => s.Id);

            builder.Property(s => s.Name)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(s => s.Interval)
                .IsRequired()
                .HasConversion<int>();

            builder.Property(s => s.Price)
                .IsRequired();

            builder.Property(s => s.StripePriceId)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(s => s.StripeProductId)
                .IsRequired()
                .HasMaxLength(200);

            builder.ToTable("Subscriptions");
        }
    }
}
