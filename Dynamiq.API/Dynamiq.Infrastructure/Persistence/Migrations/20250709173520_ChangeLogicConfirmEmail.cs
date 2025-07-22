#nullable disable

using Microsoft.EntityFrameworkCore.Migrations;

namespace Dynamiq.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class ChangeLogicConfirmEmail : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_EmailVerifications_UserId",
                table: "EmailVerifications");

            migrationBuilder.DropColumn(
                name: "ConfirmedEmail",
                table: "Users");

            migrationBuilder.RenameColumn(
                name: "IsUsed",
                table: "EmailVerifications",
                newName: "ConfirmedEmail");

            migrationBuilder.CreateIndex(
                name: "IX_EmailVerifications_UserId",
                table: "EmailVerifications",
                column: "UserId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_EmailVerifications_UserId",
                table: "EmailVerifications");

            migrationBuilder.RenameColumn(
                name: "ConfirmedEmail",
                table: "EmailVerifications",
                newName: "IsUsed");

            migrationBuilder.AddColumn<bool>(
                name: "ConfirmedEmail",
                table: "Users",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateIndex(
                name: "IX_EmailVerifications_UserId",
                table: "EmailVerifications",
                column: "UserId");
        }
    }
}
