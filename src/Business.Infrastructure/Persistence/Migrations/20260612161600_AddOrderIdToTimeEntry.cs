using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Business.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddOrderIdToTimeEntry : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "OrderId",
                table: "TimeEntries",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_TimeEntries_OrderId_UserId_ClockOut",
                table: "TimeEntries",
                columns: new[] { "OrderId", "UserId", "ClockOut" });

            migrationBuilder.AddForeignKey(
                name: "FK_TimeEntries_Orders_OrderId",
                table: "TimeEntries",
                column: "OrderId",
                principalTable: "Orders",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TimeEntries_Orders_OrderId",
                table: "TimeEntries");

            migrationBuilder.DropIndex(
                name: "IX_TimeEntries_OrderId_UserId_ClockOut",
                table: "TimeEntries");

            migrationBuilder.DropColumn(
                name: "OrderId",
                table: "TimeEntries");
        }
    }
}
