using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Business.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddMultiTenancy : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "CompanyId",
                table: "Users",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "CompanyId",
                table: "TimeEntries",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "CompanyId",
                table: "PlanningPeriods",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "CompanyId",
                table: "Orders",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "CompanyId",
                table: "OrderAttachments",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "CompanyId",
                table: "OrderAcceptances",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "CompanyId",
                table: "CompanySettings",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "CompanyId",
                table: "AbsenceRequests",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateTable(
                name: "Companies",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Companies", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Users_CompanyId",
                table: "Users",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_TimeEntries_CompanyId",
                table: "TimeEntries",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_PlanningPeriods_CompanyId",
                table: "PlanningPeriods",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_CompanyId",
                table: "Orders",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderAttachments_CompanyId",
                table: "OrderAttachments",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderAcceptances_CompanyId",
                table: "OrderAcceptances",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_CompanySettings_CompanyId",
                table: "CompanySettings",
                column: "CompanyId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AbsenceRequests_CompanyId",
                table: "AbsenceRequests",
                column: "CompanyId");

            // Backfill existing single-tenant data into one company so the FK below holds and
            // every legacy row belongs to a real tenant. No-op on an empty database (a fresh
            // install seeds its own company), so we never leave an orphan company behind.
            migrationBuilder.Sql(@"
                DO $$
                DECLARE legacy_company uuid := gen_random_uuid();
                BEGIN
                    IF EXISTS (SELECT 1 FROM ""Users"") THEN
                        INSERT INTO ""Companies"" (""Id"", ""Name"", ""CreatedAt"")
                        VALUES (legacy_company, 'Default Company', now() AT TIME ZONE 'utc');

                        UPDATE ""Users"" SET ""CompanyId"" = legacy_company;
                        UPDATE ""Orders"" SET ""CompanyId"" = legacy_company;
                        UPDATE ""TimeEntries"" SET ""CompanyId"" = legacy_company;
                        UPDATE ""AbsenceRequests"" SET ""CompanyId"" = legacy_company;
                        UPDATE ""PlanningPeriods"" SET ""CompanyId"" = legacy_company;
                        UPDATE ""OrderAttachments"" SET ""CompanyId"" = legacy_company;
                        UPDATE ""OrderAcceptances"" SET ""CompanyId"" = legacy_company;
                        UPDATE ""CompanySettings"" SET ""CompanyId"" = legacy_company;
                    END IF;
                END $$;");

            migrationBuilder.AddForeignKey(
                name: "FK_Users_Companies_CompanyId",
                table: "Users",
                column: "CompanyId",
                principalTable: "Companies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Users_Companies_CompanyId",
                table: "Users");

            migrationBuilder.DropTable(
                name: "Companies");

            migrationBuilder.DropIndex(
                name: "IX_Users_CompanyId",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_TimeEntries_CompanyId",
                table: "TimeEntries");

            migrationBuilder.DropIndex(
                name: "IX_PlanningPeriods_CompanyId",
                table: "PlanningPeriods");

            migrationBuilder.DropIndex(
                name: "IX_Orders_CompanyId",
                table: "Orders");

            migrationBuilder.DropIndex(
                name: "IX_OrderAttachments_CompanyId",
                table: "OrderAttachments");

            migrationBuilder.DropIndex(
                name: "IX_OrderAcceptances_CompanyId",
                table: "OrderAcceptances");

            migrationBuilder.DropIndex(
                name: "IX_CompanySettings_CompanyId",
                table: "CompanySettings");

            migrationBuilder.DropIndex(
                name: "IX_AbsenceRequests_CompanyId",
                table: "AbsenceRequests");

            migrationBuilder.DropColumn(
                name: "CompanyId",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "CompanyId",
                table: "TimeEntries");

            migrationBuilder.DropColumn(
                name: "CompanyId",
                table: "PlanningPeriods");

            migrationBuilder.DropColumn(
                name: "CompanyId",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "CompanyId",
                table: "OrderAttachments");

            migrationBuilder.DropColumn(
                name: "CompanyId",
                table: "OrderAcceptances");

            migrationBuilder.DropColumn(
                name: "CompanyId",
                table: "CompanySettings");

            migrationBuilder.DropColumn(
                name: "CompanyId",
                table: "AbsenceRequests");
        }
    }
}
