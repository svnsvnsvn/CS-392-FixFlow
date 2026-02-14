using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace fixflow.web.Migrations
{
    /// <inheritdoc />
    public partial class BaselineDBComplete : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AspNetRoles",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    Name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUsers",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    PassswordChangeRequired = table.Column<bool>(type: "boolean", nullable: false),
                    PasswordExpire = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UserName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    NormalizedUserName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    Email = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    NormalizedEmail = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>(type: "boolean", nullable: false),
                    PasswordHash = table.Column<string>(type: "text", nullable: true),
                    SecurityStamp = table.Column<string>(type: "text", nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "text", nullable: true),
                    PhoneNumber = table.Column<string>(type: "text", nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(type: "boolean", nullable: false),
                    TwoFactorEnabled = table.Column<bool>(type: "boolean", nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    LockoutEnabled = table.Column<bool>(type: "boolean", nullable: false),
                    AccessFailedCount = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUsers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "FfBuildingDirectory",
                columns: table => new
                {
                    LocationCode = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    LocationName = table.Column<string>(type: "text", nullable: false),
                    ComplexName = table.Column<string>(type: "text", nullable: false),
                    BuildingNumber = table.Column<int>(type: "integer", nullable: false),
                    NumUnits = table.Column<int>(type: "integer", nullable: false),
                    LocationLat = table.Column<decimal>(type: "numeric(9,6)", precision: 9, scale: 6, nullable: false),
                    LocationLon = table.Column<decimal>(type: "numeric(9,6)", precision: 9, scale: 6, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FfBuildingDirectory", x => x.LocationCode);
                    table.CheckConstraint("CK_Latitude_Range", "\"LocationLat\" >= -90 AND \"LocationLat\" <= 90");
                    table.CheckConstraint("CK_Longitude_Range", "\"LocationLon\" >= -180 AND \"LocationLon\" <= 180");
                });

            migrationBuilder.CreateTable(
                name: "FfPriorityCodes",
                columns: table => new
                {
                    Code = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    PriorityName = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FfPriorityCodes", x => x.Code);
                });

            migrationBuilder.CreateTable(
                name: "FfStatusCodes",
                columns: table => new
                {
                    Code = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    StatusName = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FfStatusCodes", x => x.Code);
                });

            migrationBuilder.CreateTable(
                name: "FfTicketTypes",
                columns: table => new
                {
                    Code = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    TypeName = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FfTicketTypes", x => x.Code);
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoleClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    RoleId = table.Column<string>(type: "text", nullable: false),
                    ClaimType = table.Column<string>(type: "text", nullable: true),
                    ClaimValue = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoleClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetRoleClaims_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<string>(type: "text", nullable: false),
                    ClaimType = table.Column<string>(type: "text", nullable: true),
                    ClaimValue = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetUserClaims_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserLogins",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(type: "text", nullable: false),
                    ProviderKey = table.Column<string>(type: "text", nullable: false),
                    ProviderDisplayName = table.Column<string>(type: "text", nullable: true),
                    UserId = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserLogins", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_AspNetUserLogins_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserRoles",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "text", nullable: false),
                    RoleId = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserTokens",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "text", nullable: false),
                    LoginProvider = table.Column<string>(type: "text", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Value = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_AspNetUserTokens_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "FfUserProfile",
                columns: table => new
                {
                    EmployeeId = table.Column<string>(type: "text", nullable: false),
                    FName = table.Column<string>(type: "text", nullable: false),
                    LName = table.Column<string>(type: "text", nullable: false),
                    LocationCode = table.Column<int>(type: "integer", nullable: false),
                    Unit = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FfUserProfile", x => x.EmployeeId);
                    table.ForeignKey(
                        name: "FK_FfUserProfile_AspNetUsers_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FfUserProfile_FfBuildingDirectory_LocationCode",
                        column: x => x.LocationCode,
                        principalTable: "FfBuildingDirectory",
                        principalColumn: "LocationCode",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "FfTicketRegister",
                columns: table => new
                {
                    TicketId = table.Column<Guid>(type: "uuid", nullable: false),
                    TicketShortCode = table.Column<string>(type: "text", nullable: false),
                    EnteredBy = table.Column<string>(type: "text", nullable: false),
                    RequestedBy = table.Column<string>(type: "text", nullable: false),
                    Location = table.Column<int>(type: "integer", nullable: false),
                    Unit = table.Column<int>(type: "integer", nullable: false),
                    TicketTroubleType = table.Column<int>(type: "integer", nullable: false),
                    TicketStatus = table.Column<int>(type: "integer", nullable: false),
                    TicketPriority = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FfTicketRegister", x => x.TicketId);
                    table.ForeignKey(
                        name: "FK_FfTicketRegister_FfBuildingDirectory_Location",
                        column: x => x.Location,
                        principalTable: "FfBuildingDirectory",
                        principalColumn: "LocationCode",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_FfTicketRegister_FfPriorityCodes_TicketPriority",
                        column: x => x.TicketPriority,
                        principalTable: "FfPriorityCodes",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_FfTicketRegister_FfStatusCodes_TicketStatus",
                        column: x => x.TicketStatus,
                        principalTable: "FfStatusCodes",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_FfTicketRegister_FfTicketTypes_TicketTroubleType",
                        column: x => x.TicketTroubleType,
                        principalTable: "FfTicketTypes",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_FfTicketRegister_FfUserProfile_EnteredBy",
                        column: x => x.EnteredBy,
                        principalTable: "FfUserProfile",
                        principalColumn: "EmployeeId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_FfTicketRegister_FfUserProfile_RequestedBy",
                        column: x => x.RequestedBy,
                        principalTable: "FfUserProfile",
                        principalColumn: "EmployeeId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "FfExternalNotes",
                columns: table => new
                {
                    XNoteId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    TicketId = table.Column<Guid>(type: "uuid", nullable: false),
                    UserEmployeeId = table.Column<string>(type: "text", nullable: true),
                    CreatedBy = table.Column<string>(type: "text", nullable: false),
                    TimeStamp = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Content = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FfExternalNotes", x => x.XNoteId);
                    table.ForeignKey(
                        name: "FK_FfExternalNotes_FfTicketRegister_TicketId",
                        column: x => x.TicketId,
                        principalTable: "FfTicketRegister",
                        principalColumn: "TicketId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_FfExternalNotes_FfUserProfile_UserEmployeeId",
                        column: x => x.UserEmployeeId,
                        principalTable: "FfUserProfile",
                        principalColumn: "EmployeeId");
                });

            migrationBuilder.CreateTable(
                name: "FfInternalNotes",
                columns: table => new
                {
                    INoteId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    TicketId = table.Column<Guid>(type: "uuid", nullable: false),
                    UserEmployeeId = table.Column<string>(type: "text", nullable: true),
                    CreatedBy = table.Column<string>(type: "text", nullable: false),
                    TimeStamp = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Content = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FfInternalNotes", x => x.INoteId);
                    table.ForeignKey(
                        name: "FK_FfInternalNotes_FfTicketRegister_TicketId",
                        column: x => x.TicketId,
                        principalTable: "FfTicketRegister",
                        principalColumn: "TicketId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_FfInternalNotes_FfUserProfile_UserEmployeeId",
                        column: x => x.UserEmployeeId,
                        principalTable: "FfUserProfile",
                        principalColumn: "EmployeeId");
                });

            migrationBuilder.CreateTable(
                name: "FfTicketFlow",
                columns: table => new
                {
                    ActionId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    TicketId = table.Column<Guid>(type: "uuid", nullable: false),
                    StatusCodeCode = table.Column<int>(type: "integer", nullable: true),
                    NewTicketStatus = table.Column<int>(type: "integer", nullable: false),
                    NewAssignee = table.Column<string>(type: "text", nullable: false),
                    TimeStamp = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FfTicketFlow", x => x.ActionId);
                    table.ForeignKey(
                        name: "FK_FfTicketFlow_FfStatusCodes_StatusCodeCode",
                        column: x => x.StatusCodeCode,
                        principalTable: "FfStatusCodes",
                        principalColumn: "Code");
                    table.ForeignKey(
                        name: "FK_FfTicketFlow_FfTicketRegister_TicketId",
                        column: x => x.TicketId,
                        principalTable: "FfTicketRegister",
                        principalColumn: "TicketId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_FfTicketFlow_FfUserProfile_NewAssignee",
                        column: x => x.NewAssignee,
                        principalTable: "FfUserProfile",
                        principalColumn: "EmployeeId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AspNetRoleClaims_RoleId",
                table: "AspNetRoleClaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "AspNetRoles",
                column: "NormalizedName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserClaims_UserId",
                table: "AspNetUserClaims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserLogins_UserId",
                table: "AspNetUserLogins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserRoles_RoleId",
                table: "AspNetUserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "AspNetUsers",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "AspNetUsers",
                column: "NormalizedUserName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_FfExternalNotes_TicketId",
                table: "FfExternalNotes",
                column: "TicketId");

            migrationBuilder.CreateIndex(
                name: "IX_FfExternalNotes_UserEmployeeId",
                table: "FfExternalNotes",
                column: "UserEmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_FfInternalNotes_TicketId",
                table: "FfInternalNotes",
                column: "TicketId");

            migrationBuilder.CreateIndex(
                name: "IX_FfInternalNotes_UserEmployeeId",
                table: "FfInternalNotes",
                column: "UserEmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_FfTicketFlow_NewAssignee",
                table: "FfTicketFlow",
                column: "NewAssignee");

            migrationBuilder.CreateIndex(
                name: "IX_FfTicketFlow_StatusCodeCode",
                table: "FfTicketFlow",
                column: "StatusCodeCode");

            migrationBuilder.CreateIndex(
                name: "IX_FfTicketFlow_TicketId",
                table: "FfTicketFlow",
                column: "TicketId");

            migrationBuilder.CreateIndex(
                name: "IX_FfTicketRegister_EnteredBy",
                table: "FfTicketRegister",
                column: "EnteredBy");

            migrationBuilder.CreateIndex(
                name: "IX_FfTicketRegister_Location",
                table: "FfTicketRegister",
                column: "Location");

            migrationBuilder.CreateIndex(
                name: "IX_FfTicketRegister_RequestedBy",
                table: "FfTicketRegister",
                column: "RequestedBy");

            migrationBuilder.CreateIndex(
                name: "IX_FfTicketRegister_TicketPriority",
                table: "FfTicketRegister",
                column: "TicketPriority");

            migrationBuilder.CreateIndex(
                name: "IX_FfTicketRegister_TicketStatus",
                table: "FfTicketRegister",
                column: "TicketStatus");

            migrationBuilder.CreateIndex(
                name: "IX_FfTicketRegister_TicketTroubleType",
                table: "FfTicketRegister",
                column: "TicketTroubleType");

            migrationBuilder.CreateIndex(
                name: "IX_FfUserProfile_LocationCode",
                table: "FfUserProfile",
                column: "LocationCode");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AspNetRoleClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserLogins");

            migrationBuilder.DropTable(
                name: "AspNetUserRoles");

            migrationBuilder.DropTable(
                name: "AspNetUserTokens");

            migrationBuilder.DropTable(
                name: "FfExternalNotes");

            migrationBuilder.DropTable(
                name: "FfInternalNotes");

            migrationBuilder.DropTable(
                name: "FfTicketFlow");

            migrationBuilder.DropTable(
                name: "AspNetRoles");

            migrationBuilder.DropTable(
                name: "FfTicketRegister");

            migrationBuilder.DropTable(
                name: "FfPriorityCodes");

            migrationBuilder.DropTable(
                name: "FfStatusCodes");

            migrationBuilder.DropTable(
                name: "FfTicketTypes");

            migrationBuilder.DropTable(
                name: "FfUserProfile");

            migrationBuilder.DropTable(
                name: "AspNetUsers");

            migrationBuilder.DropTable(
                name: "FfBuildingDirectory");
        }
    }
}
