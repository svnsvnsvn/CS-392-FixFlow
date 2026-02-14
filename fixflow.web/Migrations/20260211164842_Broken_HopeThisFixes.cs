using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace fixflow.web.Migrations
{
    /// <inheritdoc />
    public partial class Broken_HopeThisFixes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FfExternalNotes_FfTicketRegister_TicketId",
                table: "FfExternalNotes");

            migrationBuilder.DropForeignKey(
                name: "FK_FfExternalNotes_FfUserProfile_UserEmployeeId",
                table: "FfExternalNotes");

            migrationBuilder.DropForeignKey(
                name: "FK_FfInternalNotes_FfTicketRegister_TicketId",
                table: "FfInternalNotes");

            migrationBuilder.DropForeignKey(
                name: "FK_FfInternalNotes_FfUserProfile_UserEmployeeId",
                table: "FfInternalNotes");

            migrationBuilder.DropForeignKey(
                name: "FK_FfTicketFlow_FfStatusCodes_StatusCodeCode",
                table: "FfTicketFlow");

            migrationBuilder.DropForeignKey(
                name: "FK_FfTicketFlow_FfTicketRegister_TicketId",
                table: "FfTicketFlow");

            migrationBuilder.DropForeignKey(
                name: "FK_FfTicketFlow_FfUserProfile_NewAssignee",
                table: "FfTicketFlow");

            migrationBuilder.DropForeignKey(
                name: "FK_FfTicketRegister_FfBuildingDirectory_Location",
                table: "FfTicketRegister");

            migrationBuilder.DropForeignKey(
                name: "FK_FfTicketRegister_FfPriorityCodes_TicketPriority",
                table: "FfTicketRegister");

            migrationBuilder.DropForeignKey(
                name: "FK_FfTicketRegister_FfStatusCodes_TicketStatus",
                table: "FfTicketRegister");

            migrationBuilder.DropForeignKey(
                name: "FK_FfTicketRegister_FfTicketTypes_TicketTroubleType",
                table: "FfTicketRegister");

            migrationBuilder.DropForeignKey(
                name: "FK_FfTicketRegister_FfUserProfile_EnteredBy",
                table: "FfTicketRegister");

            migrationBuilder.DropForeignKey(
                name: "FK_FfTicketRegister_FfUserProfile_RequestedBy",
                table: "FfTicketRegister");

            migrationBuilder.DropForeignKey(
                name: "FK_FfUserProfile_AspNetUsers_EmployeeId",
                table: "FfUserProfile");

            migrationBuilder.DropForeignKey(
                name: "FK_FfUserProfile_FfBuildingDirectory_LocationCode",
                table: "FfUserProfile");

            migrationBuilder.DropPrimaryKey(
                name: "PK_FfUserProfile",
                table: "FfUserProfile");

            migrationBuilder.DropPrimaryKey(
                name: "PK_FfTicketTypes",
                table: "FfTicketTypes");

            migrationBuilder.DropPrimaryKey(
                name: "PK_FfTicketRegister",
                table: "FfTicketRegister");

            migrationBuilder.DropPrimaryKey(
                name: "PK_FfTicketFlow",
                table: "FfTicketFlow");

            migrationBuilder.DropPrimaryKey(
                name: "PK_FfPriorityCodes",
                table: "FfPriorityCodes");

            migrationBuilder.DropPrimaryKey(
                name: "PK_FfInternalNotes",
                table: "FfInternalNotes");

            migrationBuilder.DropPrimaryKey(
                name: "PK_FfExternalNotes",
                table: "FfExternalNotes");

            migrationBuilder.DropPrimaryKey(
                name: "PK_FfBuildingDirectory",
                table: "FfBuildingDirectory");

            migrationBuilder.RenameTable(
                name: "FfUserProfile",
                newName: "FfUserProfiles");

            migrationBuilder.RenameTable(
                name: "FfTicketTypes",
                newName: "FfTicketTypess");

            migrationBuilder.RenameTable(
                name: "FfTicketRegister",
                newName: "FfTicketRegisters");

            migrationBuilder.RenameTable(
                name: "FfTicketFlow",
                newName: "FfTicketFlows");

            migrationBuilder.RenameTable(
                name: "FfPriorityCodes",
                newName: "FfPriorityCodess");

            migrationBuilder.RenameTable(
                name: "FfInternalNotes",
                newName: "FfInternalNotess");

            migrationBuilder.RenameTable(
                name: "FfExternalNotes",
                newName: "FfExternalNotess");

            migrationBuilder.RenameTable(
                name: "FfBuildingDirectory",
                newName: "FfBuildingDirectorys");

            migrationBuilder.RenameIndex(
                name: "IX_FfUserProfile_LocationCode",
                table: "FfUserProfiles",
                newName: "IX_FfUserProfiles_LocationCode");

            migrationBuilder.RenameIndex(
                name: "IX_FfTicketRegister_TicketTroubleType",
                table: "FfTicketRegisters",
                newName: "IX_FfTicketRegisters_TicketTroubleType");

            migrationBuilder.RenameIndex(
                name: "IX_FfTicketRegister_TicketStatus",
                table: "FfTicketRegisters",
                newName: "IX_FfTicketRegisters_TicketStatus");

            migrationBuilder.RenameIndex(
                name: "IX_FfTicketRegister_TicketPriority",
                table: "FfTicketRegisters",
                newName: "IX_FfTicketRegisters_TicketPriority");

            migrationBuilder.RenameIndex(
                name: "IX_FfTicketRegister_RequestedBy",
                table: "FfTicketRegisters",
                newName: "IX_FfTicketRegisters_RequestedBy");

            migrationBuilder.RenameIndex(
                name: "IX_FfTicketRegister_Location",
                table: "FfTicketRegisters",
                newName: "IX_FfTicketRegisters_Location");

            migrationBuilder.RenameIndex(
                name: "IX_FfTicketRegister_EnteredBy",
                table: "FfTicketRegisters",
                newName: "IX_FfTicketRegisters_EnteredBy");

            migrationBuilder.RenameIndex(
                name: "IX_FfTicketFlow_TicketId",
                table: "FfTicketFlows",
                newName: "IX_FfTicketFlows_TicketId");

            migrationBuilder.RenameIndex(
                name: "IX_FfTicketFlow_StatusCodeCode",
                table: "FfTicketFlows",
                newName: "IX_FfTicketFlows_StatusCodeCode");

            migrationBuilder.RenameIndex(
                name: "IX_FfTicketFlow_NewAssignee",
                table: "FfTicketFlows",
                newName: "IX_FfTicketFlows_NewAssignee");

            migrationBuilder.RenameIndex(
                name: "IX_FfInternalNotes_UserEmployeeId",
                table: "FfInternalNotess",
                newName: "IX_FfInternalNotess_UserEmployeeId");

            migrationBuilder.RenameIndex(
                name: "IX_FfInternalNotes_TicketId",
                table: "FfInternalNotess",
                newName: "IX_FfInternalNotess_TicketId");

            migrationBuilder.RenameIndex(
                name: "IX_FfExternalNotes_UserEmployeeId",
                table: "FfExternalNotess",
                newName: "IX_FfExternalNotess_UserEmployeeId");

            migrationBuilder.RenameIndex(
                name: "IX_FfExternalNotes_TicketId",
                table: "FfExternalNotess",
                newName: "IX_FfExternalNotess_TicketId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_FfUserProfiles",
                table: "FfUserProfiles",
                column: "EmployeeId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_FfTicketTypess",
                table: "FfTicketTypess",
                column: "Code");

            migrationBuilder.AddPrimaryKey(
                name: "PK_FfTicketRegisters",
                table: "FfTicketRegisters",
                column: "TicketId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_FfTicketFlows",
                table: "FfTicketFlows",
                column: "ActionId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_FfPriorityCodess",
                table: "FfPriorityCodess",
                column: "Code");

            migrationBuilder.AddPrimaryKey(
                name: "PK_FfInternalNotess",
                table: "FfInternalNotess",
                column: "INoteId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_FfExternalNotess",
                table: "FfExternalNotess",
                column: "XNoteId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_FfBuildingDirectorys",
                table: "FfBuildingDirectorys",
                column: "LocationCode");

            migrationBuilder.AddForeignKey(
                name: "FK_FfExternalNotess_FfTicketRegisters_TicketId",
                table: "FfExternalNotess",
                column: "TicketId",
                principalTable: "FfTicketRegisters",
                principalColumn: "TicketId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_FfExternalNotess_FfUserProfiles_UserEmployeeId",
                table: "FfExternalNotess",
                column: "UserEmployeeId",
                principalTable: "FfUserProfiles",
                principalColumn: "EmployeeId");

            migrationBuilder.AddForeignKey(
                name: "FK_FfInternalNotess_FfTicketRegisters_TicketId",
                table: "FfInternalNotess",
                column: "TicketId",
                principalTable: "FfTicketRegisters",
                principalColumn: "TicketId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_FfInternalNotess_FfUserProfiles_UserEmployeeId",
                table: "FfInternalNotess",
                column: "UserEmployeeId",
                principalTable: "FfUserProfiles",
                principalColumn: "EmployeeId");

            migrationBuilder.AddForeignKey(
                name: "FK_FfTicketFlows_FfStatusCodes_StatusCodeCode",
                table: "FfTicketFlows",
                column: "StatusCodeCode",
                principalTable: "FfStatusCodes",
                principalColumn: "Code");

            migrationBuilder.AddForeignKey(
                name: "FK_FfTicketFlows_FfTicketRegisters_TicketId",
                table: "FfTicketFlows",
                column: "TicketId",
                principalTable: "FfTicketRegisters",
                principalColumn: "TicketId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_FfTicketFlows_FfUserProfiles_NewAssignee",
                table: "FfTicketFlows",
                column: "NewAssignee",
                principalTable: "FfUserProfiles",
                principalColumn: "EmployeeId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_FfTicketRegisters_FfBuildingDirectorys_Location",
                table: "FfTicketRegisters",
                column: "Location",
                principalTable: "FfBuildingDirectorys",
                principalColumn: "LocationCode",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_FfTicketRegisters_FfPriorityCodess_TicketPriority",
                table: "FfTicketRegisters",
                column: "TicketPriority",
                principalTable: "FfPriorityCodess",
                principalColumn: "Code",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_FfTicketRegisters_FfStatusCodes_TicketStatus",
                table: "FfTicketRegisters",
                column: "TicketStatus",
                principalTable: "FfStatusCodes",
                principalColumn: "Code",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_FfTicketRegisters_FfTicketTypess_TicketTroubleType",
                table: "FfTicketRegisters",
                column: "TicketTroubleType",
                principalTable: "FfTicketTypess",
                principalColumn: "Code",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_FfTicketRegisters_FfUserProfiles_EnteredBy",
                table: "FfTicketRegisters",
                column: "EnteredBy",
                principalTable: "FfUserProfiles",
                principalColumn: "EmployeeId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_FfTicketRegisters_FfUserProfiles_RequestedBy",
                table: "FfTicketRegisters",
                column: "RequestedBy",
                principalTable: "FfUserProfiles",
                principalColumn: "EmployeeId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_FfUserProfiles_AspNetUsers_EmployeeId",
                table: "FfUserProfiles",
                column: "EmployeeId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_FfUserProfiles_FfBuildingDirectorys_LocationCode",
                table: "FfUserProfiles",
                column: "LocationCode",
                principalTable: "FfBuildingDirectorys",
                principalColumn: "LocationCode",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FfExternalNotess_FfTicketRegisters_TicketId",
                table: "FfExternalNotess");

            migrationBuilder.DropForeignKey(
                name: "FK_FfExternalNotess_FfUserProfiles_UserEmployeeId",
                table: "FfExternalNotess");

            migrationBuilder.DropForeignKey(
                name: "FK_FfInternalNotess_FfTicketRegisters_TicketId",
                table: "FfInternalNotess");

            migrationBuilder.DropForeignKey(
                name: "FK_FfInternalNotess_FfUserProfiles_UserEmployeeId",
                table: "FfInternalNotess");

            migrationBuilder.DropForeignKey(
                name: "FK_FfTicketFlows_FfStatusCodes_StatusCodeCode",
                table: "FfTicketFlows");

            migrationBuilder.DropForeignKey(
                name: "FK_FfTicketFlows_FfTicketRegisters_TicketId",
                table: "FfTicketFlows");

            migrationBuilder.DropForeignKey(
                name: "FK_FfTicketFlows_FfUserProfiles_NewAssignee",
                table: "FfTicketFlows");

            migrationBuilder.DropForeignKey(
                name: "FK_FfTicketRegisters_FfBuildingDirectorys_Location",
                table: "FfTicketRegisters");

            migrationBuilder.DropForeignKey(
                name: "FK_FfTicketRegisters_FfPriorityCodess_TicketPriority",
                table: "FfTicketRegisters");

            migrationBuilder.DropForeignKey(
                name: "FK_FfTicketRegisters_FfStatusCodes_TicketStatus",
                table: "FfTicketRegisters");

            migrationBuilder.DropForeignKey(
                name: "FK_FfTicketRegisters_FfTicketTypess_TicketTroubleType",
                table: "FfTicketRegisters");

            migrationBuilder.DropForeignKey(
                name: "FK_FfTicketRegisters_FfUserProfiles_EnteredBy",
                table: "FfTicketRegisters");

            migrationBuilder.DropForeignKey(
                name: "FK_FfTicketRegisters_FfUserProfiles_RequestedBy",
                table: "FfTicketRegisters");

            migrationBuilder.DropForeignKey(
                name: "FK_FfUserProfiles_AspNetUsers_EmployeeId",
                table: "FfUserProfiles");

            migrationBuilder.DropForeignKey(
                name: "FK_FfUserProfiles_FfBuildingDirectorys_LocationCode",
                table: "FfUserProfiles");

            migrationBuilder.DropPrimaryKey(
                name: "PK_FfUserProfiles",
                table: "FfUserProfiles");

            migrationBuilder.DropPrimaryKey(
                name: "PK_FfTicketTypess",
                table: "FfTicketTypess");

            migrationBuilder.DropPrimaryKey(
                name: "PK_FfTicketRegisters",
                table: "FfTicketRegisters");

            migrationBuilder.DropPrimaryKey(
                name: "PK_FfTicketFlows",
                table: "FfTicketFlows");

            migrationBuilder.DropPrimaryKey(
                name: "PK_FfPriorityCodess",
                table: "FfPriorityCodess");

            migrationBuilder.DropPrimaryKey(
                name: "PK_FfInternalNotess",
                table: "FfInternalNotess");

            migrationBuilder.DropPrimaryKey(
                name: "PK_FfExternalNotess",
                table: "FfExternalNotess");

            migrationBuilder.DropPrimaryKey(
                name: "PK_FfBuildingDirectorys",
                table: "FfBuildingDirectorys");

            migrationBuilder.RenameTable(
                name: "FfUserProfiles",
                newName: "FfUserProfile");

            migrationBuilder.RenameTable(
                name: "FfTicketTypess",
                newName: "FfTicketTypes");

            migrationBuilder.RenameTable(
                name: "FfTicketRegisters",
                newName: "FfTicketRegister");

            migrationBuilder.RenameTable(
                name: "FfTicketFlows",
                newName: "FfTicketFlow");

            migrationBuilder.RenameTable(
                name: "FfPriorityCodess",
                newName: "FfPriorityCodes");

            migrationBuilder.RenameTable(
                name: "FfInternalNotess",
                newName: "FfInternalNotes");

            migrationBuilder.RenameTable(
                name: "FfExternalNotess",
                newName: "FfExternalNotes");

            migrationBuilder.RenameTable(
                name: "FfBuildingDirectorys",
                newName: "FfBuildingDirectory");

            migrationBuilder.RenameIndex(
                name: "IX_FfUserProfiles_LocationCode",
                table: "FfUserProfile",
                newName: "IX_FfUserProfile_LocationCode");

            migrationBuilder.RenameIndex(
                name: "IX_FfTicketRegisters_TicketTroubleType",
                table: "FfTicketRegister",
                newName: "IX_FfTicketRegister_TicketTroubleType");

            migrationBuilder.RenameIndex(
                name: "IX_FfTicketRegisters_TicketStatus",
                table: "FfTicketRegister",
                newName: "IX_FfTicketRegister_TicketStatus");

            migrationBuilder.RenameIndex(
                name: "IX_FfTicketRegisters_TicketPriority",
                table: "FfTicketRegister",
                newName: "IX_FfTicketRegister_TicketPriority");

            migrationBuilder.RenameIndex(
                name: "IX_FfTicketRegisters_RequestedBy",
                table: "FfTicketRegister",
                newName: "IX_FfTicketRegister_RequestedBy");

            migrationBuilder.RenameIndex(
                name: "IX_FfTicketRegisters_Location",
                table: "FfTicketRegister",
                newName: "IX_FfTicketRegister_Location");

            migrationBuilder.RenameIndex(
                name: "IX_FfTicketRegisters_EnteredBy",
                table: "FfTicketRegister",
                newName: "IX_FfTicketRegister_EnteredBy");

            migrationBuilder.RenameIndex(
                name: "IX_FfTicketFlows_TicketId",
                table: "FfTicketFlow",
                newName: "IX_FfTicketFlow_TicketId");

            migrationBuilder.RenameIndex(
                name: "IX_FfTicketFlows_StatusCodeCode",
                table: "FfTicketFlow",
                newName: "IX_FfTicketFlow_StatusCodeCode");

            migrationBuilder.RenameIndex(
                name: "IX_FfTicketFlows_NewAssignee",
                table: "FfTicketFlow",
                newName: "IX_FfTicketFlow_NewAssignee");

            migrationBuilder.RenameIndex(
                name: "IX_FfInternalNotess_UserEmployeeId",
                table: "FfInternalNotes",
                newName: "IX_FfInternalNotes_UserEmployeeId");

            migrationBuilder.RenameIndex(
                name: "IX_FfInternalNotess_TicketId",
                table: "FfInternalNotes",
                newName: "IX_FfInternalNotes_TicketId");

            migrationBuilder.RenameIndex(
                name: "IX_FfExternalNotess_UserEmployeeId",
                table: "FfExternalNotes",
                newName: "IX_FfExternalNotes_UserEmployeeId");

            migrationBuilder.RenameIndex(
                name: "IX_FfExternalNotess_TicketId",
                table: "FfExternalNotes",
                newName: "IX_FfExternalNotes_TicketId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_FfUserProfile",
                table: "FfUserProfile",
                column: "EmployeeId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_FfTicketTypes",
                table: "FfTicketTypes",
                column: "Code");

            migrationBuilder.AddPrimaryKey(
                name: "PK_FfTicketRegister",
                table: "FfTicketRegister",
                column: "TicketId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_FfTicketFlow",
                table: "FfTicketFlow",
                column: "ActionId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_FfPriorityCodes",
                table: "FfPriorityCodes",
                column: "Code");

            migrationBuilder.AddPrimaryKey(
                name: "PK_FfInternalNotes",
                table: "FfInternalNotes",
                column: "INoteId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_FfExternalNotes",
                table: "FfExternalNotes",
                column: "XNoteId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_FfBuildingDirectory",
                table: "FfBuildingDirectory",
                column: "LocationCode");

            migrationBuilder.AddForeignKey(
                name: "FK_FfExternalNotes_FfTicketRegister_TicketId",
                table: "FfExternalNotes",
                column: "TicketId",
                principalTable: "FfTicketRegister",
                principalColumn: "TicketId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_FfExternalNotes_FfUserProfile_UserEmployeeId",
                table: "FfExternalNotes",
                column: "UserEmployeeId",
                principalTable: "FfUserProfile",
                principalColumn: "EmployeeId");

            migrationBuilder.AddForeignKey(
                name: "FK_FfInternalNotes_FfTicketRegister_TicketId",
                table: "FfInternalNotes",
                column: "TicketId",
                principalTable: "FfTicketRegister",
                principalColumn: "TicketId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_FfInternalNotes_FfUserProfile_UserEmployeeId",
                table: "FfInternalNotes",
                column: "UserEmployeeId",
                principalTable: "FfUserProfile",
                principalColumn: "EmployeeId");

            migrationBuilder.AddForeignKey(
                name: "FK_FfTicketFlow_FfStatusCodes_StatusCodeCode",
                table: "FfTicketFlow",
                column: "StatusCodeCode",
                principalTable: "FfStatusCodes",
                principalColumn: "Code");

            migrationBuilder.AddForeignKey(
                name: "FK_FfTicketFlow_FfTicketRegister_TicketId",
                table: "FfTicketFlow",
                column: "TicketId",
                principalTable: "FfTicketRegister",
                principalColumn: "TicketId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_FfTicketFlow_FfUserProfile_NewAssignee",
                table: "FfTicketFlow",
                column: "NewAssignee",
                principalTable: "FfUserProfile",
                principalColumn: "EmployeeId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_FfTicketRegister_FfBuildingDirectory_Location",
                table: "FfTicketRegister",
                column: "Location",
                principalTable: "FfBuildingDirectory",
                principalColumn: "LocationCode",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_FfTicketRegister_FfPriorityCodes_TicketPriority",
                table: "FfTicketRegister",
                column: "TicketPriority",
                principalTable: "FfPriorityCodes",
                principalColumn: "Code",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_FfTicketRegister_FfStatusCodes_TicketStatus",
                table: "FfTicketRegister",
                column: "TicketStatus",
                principalTable: "FfStatusCodes",
                principalColumn: "Code",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_FfTicketRegister_FfTicketTypes_TicketTroubleType",
                table: "FfTicketRegister",
                column: "TicketTroubleType",
                principalTable: "FfTicketTypes",
                principalColumn: "Code",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_FfTicketRegister_FfUserProfile_EnteredBy",
                table: "FfTicketRegister",
                column: "EnteredBy",
                principalTable: "FfUserProfile",
                principalColumn: "EmployeeId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_FfTicketRegister_FfUserProfile_RequestedBy",
                table: "FfTicketRegister",
                column: "RequestedBy",
                principalTable: "FfUserProfile",
                principalColumn: "EmployeeId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_FfUserProfile_AspNetUsers_EmployeeId",
                table: "FfUserProfile",
                column: "EmployeeId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_FfUserProfile_FfBuildingDirectory_LocationCode",
                table: "FfUserProfile",
                column: "LocationCode",
                principalTable: "FfBuildingDirectory",
                principalColumn: "LocationCode",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
