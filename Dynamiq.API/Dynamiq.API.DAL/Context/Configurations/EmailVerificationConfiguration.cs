using Dynamiq.API.DAL.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Dynamiq.API.DAL.Context.Configurations
{
    public class EmailVerificationConfiguration : IEntityTypeConfiguration<EmailVerification>
    {
        public void Configure(EntityTypeBuilder<EmailVerification> builder)
        {
            builder.HasKey(ev => ev.Id);

            builder.Property(ev => ev.Token)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(ev => ev.ExpiresAt)
                .IsRequired();

            builder.Property(ev => ev.ConfirmedEmail)
                .IsRequired();

            builder.HasOne(ev => ev.User)
                   .WithOne(u => u.EmailVerification)
                   .HasForeignKey<EmailVerification>(ev => ev.UserId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
