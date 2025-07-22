#nullable disable

using Microsoft.EntityFrameworkCore.Migrations;

namespace Dynamiq.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddPaymentTypeToProduct : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "PaymentType",
                table: "Products",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PaymentType",
                table: "Products");
        }
    }
}
