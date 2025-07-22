#nullable disable

using Microsoft.EntityFrameworkCore.Migrations;

namespace Dynamiq.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class ChangePropProductNames : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "StripePrice",
                table: "Products",
                newName: "StripeProductId");

            migrationBuilder.RenameColumn(
                name: "StripeId",
                table: "Products",
                newName: "StripePriceId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "StripeProductId",
                table: "Products",
                newName: "StripePrice");

            migrationBuilder.RenameColumn(
                name: "StripePriceId",
                table: "Products",
                newName: "StripeId");
        }
    }
}
