using Dynamiq.Domain.Aggregates;
using Dynamiq.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Dynamiq.Infrastructure.Persistence.Configurations
{
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.HasKey(u => u.Id);

            builder.HasIndex(u => u.Email).IsUnique();

            builder.Property(u => u.Email)
                   .IsRequired();

            builder.Property(u => u.PasswordHash)
                   .IsRequired();

            builder.Property(u => u.Role)
                   .IsRequired();

            builder.HasOne(u => u.RefreshToken)
                   .WithOne()
                   .HasForeignKey<RefreshToken>(rt => rt.UserId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(u => u.PaymentHistories)
                   .WithOne()
                   .HasForeignKey(ph => ph.UserId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(u => u.Subscriptions)
                   .WithOne()
                   .HasForeignKey(s => s.UserId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(u => u.EmailVerification)
                   .WithOne()
                   .HasForeignKey<EmailVerification>(ev => ev.UserId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
