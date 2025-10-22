using Dynamiq.Domain.Aggregates;
using Dynamiq.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Dynamiq.Infrastructure.Persistence.Configurations
{
    public class ProductConfiguration : IEntityTypeConfiguration<Product>
    {
        public void Configure(EntityTypeBuilder<Product> builder)
        {
            builder.HasKey(p => p.Id);
            builder.Property(p => p.Id)
                   .ValueGeneratedOnAdd()
                   .HasDefaultValueSql("NEWID()");

            builder.Property(p => p.StripeProductId)
                   .IsRequired()
                   .HasMaxLength(100);

            builder.Property(p => p.StripePriceId)
                   .IsRequired()
                   .HasMaxLength(100);

            builder.Property(p => p.Name)
                   .IsRequired()
                   .HasMaxLength(200);

            builder.Property(p => p.Description)
                   .HasMaxLength(1000);

            builder.Property(p => p.Price)
                   .IsRequired();

            builder.Property(p => p.ViewCount)
                   .IsRequired();

            builder.Property(p => p.CategoryId)
                   .IsRequired();

            builder.Property(p => p.CardDescription)
                   .HasMaxLength(35);

            builder.HasMany<ProductPaymentHistory>()
                   .WithOne()
                   .HasForeignKey(ph => ph.ProductId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany<CartItem>()
                .WithOne()
                .HasForeignKey(ci => ci.ProductId)
                .OnDelete(DeleteBehavior.Restrict);


            builder.OwnsMany(p => p.ImgUrls, imgBuilder =>
            {
                imgBuilder.ToTable("ProductImgUrls");

                imgBuilder.WithOwner()
                          .HasForeignKey("ProductId");

                imgBuilder.Property<Guid>("Id")
                          .ValueGeneratedOnAdd()
                          .HasDefaultValueSql("NEWID()");

                imgBuilder.HasKey("Id");

                imgBuilder.Property(i => i.ImgUrl)
                          .IsRequired()
                          .HasMaxLength(500);
            });

            builder.OwnsMany(p => p.Paragraphs, paragraphBuilder =>
            {
                paragraphBuilder.ToTable("ProductParagraphs");

                paragraphBuilder.WithOwner()
                                .HasForeignKey("ProductId");

                paragraphBuilder.Property<int>("Id")
                                .ValueGeneratedOnAdd();

                paragraphBuilder.HasKey("Id");

                paragraphBuilder.Property(p => p.Text)
                                .IsRequired()
                                .HasMaxLength(2000);

                paragraphBuilder.Property(p => p.Order)
                                .IsRequired();
            });

            builder.HasOne<Category>()
                   .WithMany()
                   .HasForeignKey(p => p.CategoryId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
