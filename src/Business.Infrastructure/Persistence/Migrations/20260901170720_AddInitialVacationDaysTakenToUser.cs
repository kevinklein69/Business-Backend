using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Business.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddInitialVacationDaysTakenToUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "InitialVacationDaysTaken",
                table: "Users",
                type: "numeric",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "InitialVacationYear",
                table: "Users",
                type: "integer",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "InitialVacationDaysTaken",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "InitialVacationYear",
                table: "Users");
        }
    }
}
