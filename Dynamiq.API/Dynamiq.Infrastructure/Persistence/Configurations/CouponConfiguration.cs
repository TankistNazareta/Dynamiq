using Dynamiq.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Dynamiq.Infrastructure.Persistence.Configurations
{
    public class CouponConfiguration : IEntityTypeConfiguration<Coupon>
    {
        public void Configure(EntityTypeBuilder<Coupon> builder)
        {
            builder.HasKey(c => c.Id);
            builder.Property(c => c.Id)
               .ValueGeneratedOnAdd()
               .HasDefaultValueSql("NEWID()");

            builder.Property(c => c.Code)
                .IsRequired()
                .IsUnicode()
                .HasMaxLength(50);

            builder.Property(c => c.DiscountType)
                .IsRequired()
                .HasConversion<int>();

            builder.Property(c => c.DiscountValue)
                .IsRequired();

            builder.Property(c => c.StartTime)
                .IsRequired();

            builder.Property(c => c.EndTime)
                .IsRequired();

            builder.Property(c => c.IsActiveCoupon)
                .HasColumnName("IsActive")
                .IsRequired();

            builder.ToTable("Coupons");
        }
    }
}
