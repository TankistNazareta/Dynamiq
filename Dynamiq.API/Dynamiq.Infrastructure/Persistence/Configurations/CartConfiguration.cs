using Dynamiq.Domain.Aggregates;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Dynamiq.Infrastructure.Persistence.Configurations
{
    public class CartConfiguration : IEntityTypeConfiguration<Cart>
    {
        public void Configure(EntityTypeBuilder<Cart> builder)
        {
            builder.ToTable("Carts");

            builder.HasKey(c => c.Id);
            builder.Property(c => c.Id)
                   .ValueGeneratedOnAdd()
                   .HasDefaultValueSql("NEWID()");

            builder.Property(c => c.UserId)
                .IsRequired();

            builder.OwnsMany(c => c.Items, items =>
            {
                items.WithOwner().HasForeignKey("CartId");

                items.ToTable("CartItems");

                items.Property<Guid>("Id");
                items.HasKey("Id");

                items.Property(i => i.ProductId)
                    .IsRequired();

                items.Property(i => i.Quantity)
                    .IsRequired();
            });
        }
    }
}
