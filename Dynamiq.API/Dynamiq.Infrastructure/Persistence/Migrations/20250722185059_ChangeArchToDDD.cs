using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Dynamiq.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class ChangeArchToDDD : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "Subscriptions");

            migrationBuilder.RenameColumn(
                name: "ConfirmedEmail",
                table: "EmailVerifications",
                newName: "IsConfirmed");

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "Users",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(255)",
                oldMaxLength: 255);

            migrationBuilder.AddColumn<Guid>(
                name: "PaymentHistoryId",
                table: "Subscriptions",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_Subscriptions_PaymentHistoryId",
                table: "Subscriptions",
                column: "PaymentHistoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_Subscriptions_PaymentHistories_PaymentHistoryId",
                table: "Subscriptions",
                column: "PaymentHistoryId",
                principalTable: "PaymentHistories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Subscriptions_PaymentHistories_PaymentHistoryId",
                table: "Subscriptions");

            migrationBuilder.DropIndex(
                name: "IX_Subscriptions_PaymentHistoryId",
                table: "Subscriptions");

            migrationBuilder.DropColumn(
                name: "PaymentHistoryId",
                table: "Subscriptions");

            migrationBuilder.RenameColumn(
                name: "IsConfirmed",
                table: "EmailVerifications",
                newName: "ConfirmedEmail");

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "Users",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "Subscriptions",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}
