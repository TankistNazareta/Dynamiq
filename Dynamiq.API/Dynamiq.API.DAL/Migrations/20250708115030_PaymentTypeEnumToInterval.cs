using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Dynamiq.API.DAL.Migrations
{
    /// <inheritdoc />
    public partial class PaymentTypeEnumToInterval : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "PaymentType",
                table: "Products",
                newName: "Interval");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Interval",
                table: "Products",
                newName: "PaymentType");
        }
    }
}
