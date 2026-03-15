using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace fixflow.web.Migrations
{
    /// <inheritdoc />
    public partial class StatusAndPrioirytCodeEnforceUniqueness : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "TypeName",
                table: "FfTicketTypess",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.CreateIndex(
                name: "IX_FfTicketConstructoror_SeriesIsActive",
                table: "FfTicketConstructoror",
                column: "SeriesIsActive",
                unique: true,
                filter: "\"SeriesIsActive\" = TRUE");

            migrationBuilder.CreateIndex(
                name: "IX_FfStatusCodes_StatusCode",
                table: "FfStatusCodes",
                column: "StatusCode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_FfPriorityCodess_PriorityCode",
                table: "FfPriorityCodess",
                column: "PriorityCode",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_FfTicketConstructoror_SeriesIsActive",
                table: "FfTicketConstructoror");

            migrationBuilder.DropIndex(
                name: "IX_FfStatusCodes_StatusCode",
                table: "FfStatusCodes");

            migrationBuilder.DropIndex(
                name: "IX_FfPriorityCodess_PriorityCode",
                table: "FfPriorityCodess");

            migrationBuilder.AlterColumn<string>(
                name: "TypeName",
                table: "FfTicketTypess",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DueDate",
                table: "FfTicketRegisters",
                type: "timestamp with time zone",
                nullable: true);
        }
    }
}
