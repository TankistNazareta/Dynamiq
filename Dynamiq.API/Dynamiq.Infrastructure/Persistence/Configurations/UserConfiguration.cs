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
            builder.Property(rt => rt.Id)
                   .ValueGeneratedOnAdd()
                   .HasDefaultValueSql("NEWID()");

            builder.HasIndex(u => u.Email).IsUnique();

            builder.Property(u => u.Email)
                   .IsRequired();

            builder.Property(u => u.PasswordHash)
                   .IsRequired();

            builder.Property(u => u.Role)
                   .IsRequired();

            builder.HasMany(u => u.RefreshTokens)
                   .WithOne()
                   .HasForeignKey(rt => rt.UserId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(u => u.PaymentHistories)
                   .WithOne()
                   .HasForeignKey(ph => ph.UserId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(u => u.EmailVerification)
                   .WithOne()
                   .HasForeignKey<EmailVerification>(ev => ev.UserId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
