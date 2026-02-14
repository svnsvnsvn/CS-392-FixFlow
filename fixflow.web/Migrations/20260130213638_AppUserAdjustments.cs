using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace fixflow.web.Migrations
{
    /// <inheritdoc />
    public partial class AppUserAdjustments : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "PassswordChangeRequired",
                table: "AspNetUsers",
                newName: "ResetPassOnLogin");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ResetPassOnLogin",
                table: "AspNetUsers",
                newName: "PassswordChangeRequired");
        }
    }
}
