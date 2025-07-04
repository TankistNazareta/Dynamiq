using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Dynamiq.API.DAL.Migrations
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
