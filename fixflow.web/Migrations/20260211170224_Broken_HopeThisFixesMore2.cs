using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace fixflow.web.Migrations
{
    /// <inheritdoc />
    public partial class Broken_HopeThisFixesMore2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FfExternalNotess_FfUserProfiles_UserEmployeeId",
                table: "FfExternalNotess");

            migrationBuilder.DropForeignKey(
                name: "FK_FfInternalNotess_FfUserProfiles_UserEmployeeId",
                table: "FfInternalNotess");

            migrationBuilder.DropForeignKey(
                name: "FK_FfUserProfiles_AspNetUsers_EmployeeId",
                table: "FfUserProfiles");

            migrationBuilder.RenameColumn(
                name: "EmployeeId",
                table: "FfUserProfiles",
                newName: "FfUserId");

            migrationBuilder.RenameColumn(
                name: "UserEmployeeId",
                table: "FfInternalNotess",
                newName: "UserFfUserId");

            migrationBuilder.RenameIndex(
                name: "IX_FfInternalNotess_UserEmployeeId",
                table: "FfInternalNotess",
                newName: "IX_FfInternalNotess_UserFfUserId");

            migrationBuilder.RenameColumn(
                name: "UserEmployeeId",
                table: "FfExternalNotess",
                newName: "UserFfUserId");

            migrationBuilder.RenameIndex(
                name: "IX_FfExternalNotess_UserEmployeeId",
                table: "FfExternalNotess",
                newName: "IX_FfExternalNotess_UserFfUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_FfExternalNotess_FfUserProfiles_UserFfUserId",
                table: "FfExternalNotess",
                column: "UserFfUserId",
                principalTable: "FfUserProfiles",
                principalColumn: "FfUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_FfInternalNotess_FfUserProfiles_UserFfUserId",
                table: "FfInternalNotess",
                column: "UserFfUserId",
                principalTable: "FfUserProfiles",
                principalColumn: "FfUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_FfUserProfiles_AspNetUsers_FfUserId",
                table: "FfUserProfiles",
                column: "FfUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FfExternalNotess_FfUserProfiles_UserFfUserId",
                table: "FfExternalNotess");

            migrationBuilder.DropForeignKey(
                name: "FK_FfInternalNotess_FfUserProfiles_UserFfUserId",
                table: "FfInternalNotess");

            migrationBuilder.DropForeignKey(
                name: "FK_FfUserProfiles_AspNetUsers_FfUserId",
                table: "FfUserProfiles");

            migrationBuilder.RenameColumn(
                name: "FfUserId",
                table: "FfUserProfiles",
                newName: "EmployeeId");

            migrationBuilder.RenameColumn(
                name: "UserFfUserId",
                table: "FfInternalNotess",
                newName: "UserEmployeeId");

            migrationBuilder.RenameIndex(
                name: "IX_FfInternalNotess_UserFfUserId",
                table: "FfInternalNotess",
                newName: "IX_FfInternalNotess_UserEmployeeId");

            migrationBuilder.RenameColumn(
                name: "UserFfUserId",
                table: "FfExternalNotess",
                newName: "UserEmployeeId");

            migrationBuilder.RenameIndex(
                name: "IX_FfExternalNotess_UserFfUserId",
                table: "FfExternalNotess",
                newName: "IX_FfExternalNotess_UserEmployeeId");

            migrationBuilder.AddForeignKey(
                name: "FK_FfExternalNotess_FfUserProfiles_UserEmployeeId",
                table: "FfExternalNotess",
                column: "UserEmployeeId",
                principalTable: "FfUserProfiles",
                principalColumn: "EmployeeId");

            migrationBuilder.AddForeignKey(
                name: "FK_FfInternalNotess_FfUserProfiles_UserEmployeeId",
                table: "FfInternalNotess",
                column: "UserEmployeeId",
                principalTable: "FfUserProfiles",
                principalColumn: "EmployeeId");

            migrationBuilder.AddForeignKey(
                name: "FK_FfUserProfiles_AspNetUsers_EmployeeId",
                table: "FfUserProfiles",
                column: "EmployeeId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
