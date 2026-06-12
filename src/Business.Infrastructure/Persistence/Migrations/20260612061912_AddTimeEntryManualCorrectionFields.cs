using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Business.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddTimeEntryManualCorrectionFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsManual",
                table: "TimeEntries",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "Note",
                table: "TimeEntries",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "TimeEntries",
                type: "character varying(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "Approved");

            migrationBuilder.CreateIndex(
                name: "IX_TimeEntries_UserId_Status",
                table: "TimeEntries",
                columns: new[] { "UserId", "Status" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_TimeEntries_UserId_Status",
                table: "TimeEntries");

            migrationBuilder.DropColumn(
                name: "IsManual",
                table: "TimeEntries");

            migrationBuilder.DropColumn(
                name: "Note",
                table: "TimeEntries");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "TimeEntries");
        }
    }
}
