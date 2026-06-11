using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Business.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddPlanningPeriods : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "PlanningPeriodId",
                table: "Orders",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "PlanningPeriods",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: false),
                    StartDate = table.Column<DateOnly>(type: "date", nullable: true),
                    EndDate = table.Column<DateOnly>(type: "date", nullable: true),
                    Status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlanningPeriods", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Orders_PlanningPeriodId",
                table: "Orders",
                column: "PlanningPeriodId");

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_PlanningPeriods_PlanningPeriodId",
                table: "Orders",
                column: "PlanningPeriodId",
                principalTable: "PlanningPeriods",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Orders_PlanningPeriods_PlanningPeriodId",
                table: "Orders");

            migrationBuilder.DropTable(
                name: "PlanningPeriods");

            migrationBuilder.DropIndex(
                name: "IX_Orders_PlanningPeriodId",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "PlanningPeriodId",
                table: "Orders");
        }
    }
}
