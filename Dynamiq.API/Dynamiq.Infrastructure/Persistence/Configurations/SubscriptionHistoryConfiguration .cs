using Dynamiq.Domain.Aggregates;
using Dynamiq.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Dynamiq.Infrastructure.Persistence.Configurations
{
    public class SubscriptionHistoryConfiguration : IEntityTypeConfiguration<SubscriptionHistory>
    {
        public void Configure(EntityTypeBuilder<SubscriptionHistory> builder)
        {
            builder.HasKey(sh => sh.Id);

            builder.Property(sh => sh.StartDate)
                .IsRequired();

            builder.Property(sh => sh.SubscriptionId)
                .IsRequired();

            builder.Property(sh => sh.PaymentHistoryId)
                .IsRequired();

            builder.ToTable("SubscriptionHistories");
        }
    }
}
