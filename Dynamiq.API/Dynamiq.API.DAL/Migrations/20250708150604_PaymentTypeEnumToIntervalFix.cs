using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Dynamiq.API.DAL.Migrations
{
    /// <inheritdoc />
    public partial class PaymentTypeEnumToIntervalFix : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "PaymentType",
                table: "PaymentHistories",
                newName: "Interval");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Interval",
                table: "PaymentHistories",
                newName: "PaymentType");
        }
    }
}
