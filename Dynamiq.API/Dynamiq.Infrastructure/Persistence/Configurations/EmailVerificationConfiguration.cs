using Dynamiq.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Dynamiq.Infrastructure.Persistence.Configurations
{
    public class EmailVerificationConfiguration : IEntityTypeConfiguration<EmailVerification>
    {
        public void Configure(EntityTypeBuilder<EmailVerification> builder)
        {
            builder.HasKey(ev => ev.Id);

            builder.Property(ev => ev.Token)
                .IsRequired()
                .HasMaxLength(100);

            builder.HasIndex(ev => ev.Token).IsUnique();

            builder.Property(ev => ev.ExpiresAt)
                .IsRequired();

            builder.Property(ev => ev.IsConfirmed)
                .IsRequired();

            builder.Property(ev => ev.UserId)
                .IsRequired();

            builder.HasOne<User>()
                   .WithOne(u => u.EmailVerification)
                   .HasForeignKey<EmailVerification>(ev => ev.UserId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
