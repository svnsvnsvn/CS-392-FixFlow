using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace fixflow.web.Migrations
{
    /// <inheritdoc />
    public partial class AddTicketShortCodeConstructorAndUniqueShortcode : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "TicketDescription",
                table: "FfTicketRegisters",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "TicketSubject",
                table: "FfTicketRegisters",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "FfTicketConstructoror",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    TicketPrefix = table.Column<string>(type: "text", nullable: false),
                    SeriesIsActive = table.Column<bool>(type: "boolean", nullable: false),
                    TicketSeries = table.Column<short>(type: "smallint", nullable: false),
                    LastTicketUsed = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FfTicketConstructoror", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_FfTicketRegisters_TicketShortCode",
                table: "FfTicketRegisters",
                column: "TicketShortCode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_FfTicketConstructoror_TicketSeries",
                table: "FfTicketConstructoror",
                column: "TicketSeries",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FfTicketConstructoror");

            migrationBuilder.DropIndex(
                name: "IX_FfTicketRegisters_TicketShortCode",
                table: "FfTicketRegisters");

            migrationBuilder.DropColumn(
                name: "TicketDescription",
                table: "FfTicketRegisters");

            migrationBuilder.DropColumn(
                name: "TicketSubject",
                table: "FfTicketRegisters");
        }
    }
}
