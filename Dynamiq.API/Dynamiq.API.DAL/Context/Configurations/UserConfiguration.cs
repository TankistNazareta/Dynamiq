using Dynamiq.API.DAL.Models;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace Dynamiq.API.DAL.Context.Configurations
{
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.HasKey(u => u.Id);

            builder.Property(u => u.Email)
                   .IsRequired()
                   .HasMaxLength(255);

            builder.Property(u => u.PasswordHash)
                   .IsRequired();

            builder.Property(u => u.Role)
                   .IsRequired();

            builder.HasOne(u => u.RefreshToken)
                   .WithOne(rt => rt.User)
                   .HasForeignKey<RefreshToken>(rt => rt.UserId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(u => u.PaymentHistories)
                   .WithOne(ph => ph.User)
                   .HasForeignKey(ph => ph.UserId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(u => u.Subscriptions)
                    .WithOne(s => s.User)
                    .HasForeignKey(s => s.UserId)
                    .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(u => u.EmailVerification)
                   .WithOne(ev => ev.User)
                   .HasForeignKey<EmailVerification>(ev => ev.UserId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }

}
