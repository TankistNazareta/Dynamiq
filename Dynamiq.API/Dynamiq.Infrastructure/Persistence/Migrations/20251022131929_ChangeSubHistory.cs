using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Dynamiq.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class ChangeSubHistory : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SubscriptionHistories_Users_UserId",
                table: "SubscriptionHistories");

            migrationBuilder.DropIndex(
                name: "IX_SubscriptionHistories_PaymentHistoryId",
                table: "SubscriptionHistories");

            migrationBuilder.DropIndex(
                name: "IX_SubscriptionHistories_UserId",
                table: "SubscriptionHistories");

            migrationBuilder.DropColumn(
                name: "EndDate",
                table: "SubscriptionHistories");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "SubscriptionHistories");

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "SubscriptionHistories",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateIndex(
                name: "IX_SubscriptionHistories_PaymentHistoryId",
                table: "SubscriptionHistories",
                column: "PaymentHistoryId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_SubscriptionHistories_PaymentHistoryId",
                table: "SubscriptionHistories");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "SubscriptionHistories");

            migrationBuilder.AddColumn<DateTime>(
                name: "EndDate",
                table: "SubscriptionHistories",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<Guid>(
                name: "UserId",
                table: "SubscriptionHistories",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_SubscriptionHistories_PaymentHistoryId",
                table: "SubscriptionHistories",
                column: "PaymentHistoryId");

            migrationBuilder.CreateIndex(
                name: "IX_SubscriptionHistories_UserId",
                table: "SubscriptionHistories",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_SubscriptionHistories_Users_UserId",
                table: "SubscriptionHistories",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id");
        }
    }
}
