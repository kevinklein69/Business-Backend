using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Business.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddOrderRevenueAndTimeForecast : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "ActualHours",
                table: "Orders",
                type: "numeric(7,2)",
                precision: 7,
                scale: 2,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeviationReason",
                table: "Orders",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "EstimatedHours",
                table: "Orders",
                type: "numeric(7,2)",
                precision: 7,
                scale: 2,
                nullable: true);

            migrationBuilder.AddColumn<DateOnly>(
                name: "InvoiceDate",
                table: "Orders",
                type: "date",
                nullable: true);

            migrationBuilder.AddColumn<DateOnly>(
                name: "PlannedEndDate",
                table: "Orders",
                type: "date",
                nullable: true);

            migrationBuilder.AddColumn<DateOnly>(
                name: "PlannedStartDate",
                table: "Orders",
                type: "date",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "Revenue",
                table: "Orders",
                type: "numeric(12,2)",
                precision: 12,
                scale: 2,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ActualHours",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "DeviationReason",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "EstimatedHours",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "InvoiceDate",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "PlannedEndDate",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "PlannedStartDate",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "Revenue",
                table: "Orders");
        }
    }
}
