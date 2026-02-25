using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace fixflow.web.Migrations
{
    /// <inheritdoc />
    public partial class NewBaseline : Migration
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
                    ResetPassOnLogin = table.Column<bool>(type: "boolean", nullable: false),
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
                name: "FfBuildingDirectorys",
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
                    table.PrimaryKey("PK_FfBuildingDirectorys", x => x.LocationCode);
                    table.CheckConstraint("CK_Latitude_Range", "\"LocationLat\" >= -90 AND \"LocationLat\" <= 90");
                    table.CheckConstraint("CK_Longitude_Range", "\"LocationLon\" >= -180 AND \"LocationLon\" <= 180");
                });

            migrationBuilder.CreateTable(
                name: "FfPriorityCodess",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    PriorityCode = table.Column<int>(type: "integer", nullable: false),
                    PriorityName = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FfPriorityCodess", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "FfStatusCodes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    StatusCode = table.Column<int>(type: "integer", nullable: false),
                    StatusName = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FfStatusCodes", x => x.Id);
                });

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

            migrationBuilder.CreateTable(
                name: "FfTicketTypess",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    TypeName = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FfTicketTypess", x => x.Id);
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
                name: "FfUserProfiles",
                columns: table => new
                {
                    FfUserId = table.Column<string>(type: "text", nullable: false),
                    FName = table.Column<string>(type: "text", nullable: false),
                    LName = table.Column<string>(type: "text", nullable: false),
                    LocationCode = table.Column<int>(type: "integer", nullable: false),
                    Unit = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FfUserProfiles", x => x.FfUserId);
                    table.ForeignKey(
                        name: "FK_FfUserProfiles_AspNetUsers_FfUserId",
                        column: x => x.FfUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FfUserProfiles_FfBuildingDirectorys_LocationCode",
                        column: x => x.LocationCode,
                        principalTable: "FfBuildingDirectorys",
                        principalColumn: "LocationCode",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "FfTicketRegisters",
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
                    TicketPriority = table.Column<int>(type: "integer", nullable: false),
                    TicketSubject = table.Column<string>(type: "text", nullable: false),
                    TicketDescription = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FfTicketRegisters", x => x.TicketId);
                    table.ForeignKey(
                        name: "FK_FfTicketRegisters_FfBuildingDirectorys_Location",
                        column: x => x.Location,
                        principalTable: "FfBuildingDirectorys",
                        principalColumn: "LocationCode",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_FfTicketRegisters_FfPriorityCodess_TicketPriority",
                        column: x => x.TicketPriority,
                        principalTable: "FfPriorityCodess",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_FfTicketRegisters_FfStatusCodes_TicketStatus",
                        column: x => x.TicketStatus,
                        principalTable: "FfStatusCodes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_FfTicketRegisters_FfTicketTypess_TicketTroubleType",
                        column: x => x.TicketTroubleType,
                        principalTable: "FfTicketTypess",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_FfTicketRegisters_FfUserProfiles_EnteredBy",
                        column: x => x.EnteredBy,
                        principalTable: "FfUserProfiles",
                        principalColumn: "FfUserId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_FfTicketRegisters_FfUserProfiles_RequestedBy",
                        column: x => x.RequestedBy,
                        principalTable: "FfUserProfiles",
                        principalColumn: "FfUserId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "FfExternalNotess",
                columns: table => new
                {
                    XNoteId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    TicketId = table.Column<Guid>(type: "uuid", nullable: false),
                    UserFfUserId = table.Column<string>(type: "text", nullable: true),
                    CreatedBy = table.Column<string>(type: "text", nullable: false),
                    TimeStamp = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Content = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FfExternalNotess", x => x.XNoteId);
                    table.ForeignKey(
                        name: "FK_FfExternalNotess_FfTicketRegisters_TicketId",
                        column: x => x.TicketId,
                        principalTable: "FfTicketRegisters",
                        principalColumn: "TicketId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_FfExternalNotess_FfUserProfiles_UserFfUserId",
                        column: x => x.UserFfUserId,
                        principalTable: "FfUserProfiles",
                        principalColumn: "FfUserId");
                });

            migrationBuilder.CreateTable(
                name: "FfInternalNotess",
                columns: table => new
                {
                    INoteId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    TicketId = table.Column<Guid>(type: "uuid", nullable: false),
                    UserFfUserId = table.Column<string>(type: "text", nullable: true),
                    CreatedBy = table.Column<string>(type: "text", nullable: false),
                    TimeStamp = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Content = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FfInternalNotess", x => x.INoteId);
                    table.ForeignKey(
                        name: "FK_FfInternalNotess_FfTicketRegisters_TicketId",
                        column: x => x.TicketId,
                        principalTable: "FfTicketRegisters",
                        principalColumn: "TicketId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_FfInternalNotess_FfUserProfiles_UserFfUserId",
                        column: x => x.UserFfUserId,
                        principalTable: "FfUserProfiles",
                        principalColumn: "FfUserId");
                });

            migrationBuilder.CreateTable(
                name: "FfTicketFlows",
                columns: table => new
                {
                    ActionId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    TicketId = table.Column<Guid>(type: "uuid", nullable: false),
                    StatusCodeId = table.Column<int>(type: "integer", nullable: true),
                    NewTicketStatus = table.Column<int>(type: "integer", nullable: false),
                    NewAssignee = table.Column<string>(type: "text", nullable: false),
                    TimeStamp = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FfTicketFlows", x => x.ActionId);
                    table.ForeignKey(
                        name: "FK_FfTicketFlows_FfStatusCodes_StatusCodeId",
                        column: x => x.StatusCodeId,
                        principalTable: "FfStatusCodes",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_FfTicketFlows_FfTicketRegisters_TicketId",
                        column: x => x.TicketId,
                        principalTable: "FfTicketRegisters",
                        principalColumn: "TicketId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_FfTicketFlows_FfUserProfiles_NewAssignee",
                        column: x => x.NewAssignee,
                        principalTable: "FfUserProfiles",
                        principalColumn: "FfUserId",
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
                name: "IX_FfBuildingDirectorys_LocationName",
                table: "FfBuildingDirectorys",
                column: "LocationName",
                unique: true,
                filter: "\"LocationName\" = 'Unassigned'");

            migrationBuilder.CreateIndex(
                name: "IX_FfExternalNotess_TicketId",
                table: "FfExternalNotess",
                column: "TicketId");

            migrationBuilder.CreateIndex(
                name: "IX_FfExternalNotess_UserFfUserId",
                table: "FfExternalNotess",
                column: "UserFfUserId");

            migrationBuilder.CreateIndex(
                name: "IX_FfInternalNotess_TicketId",
                table: "FfInternalNotess",
                column: "TicketId");

            migrationBuilder.CreateIndex(
                name: "IX_FfInternalNotess_UserFfUserId",
                table: "FfInternalNotess",
                column: "UserFfUserId");

            migrationBuilder.CreateIndex(
                name: "IX_FfTicketConstructoror_TicketSeries",
                table: "FfTicketConstructoror",
                column: "TicketSeries",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_FfTicketFlows_NewAssignee",
                table: "FfTicketFlows",
                column: "NewAssignee");

            migrationBuilder.CreateIndex(
                name: "IX_FfTicketFlows_StatusCodeId",
                table: "FfTicketFlows",
                column: "StatusCodeId");

            migrationBuilder.CreateIndex(
                name: "IX_FfTicketFlows_TicketId",
                table: "FfTicketFlows",
                column: "TicketId");

            migrationBuilder.CreateIndex(
                name: "IX_FfTicketRegisters_EnteredBy",
                table: "FfTicketRegisters",
                column: "EnteredBy");

            migrationBuilder.CreateIndex(
                name: "IX_FfTicketRegisters_Location",
                table: "FfTicketRegisters",
                column: "Location");

            migrationBuilder.CreateIndex(
                name: "IX_FfTicketRegisters_RequestedBy",
                table: "FfTicketRegisters",
                column: "RequestedBy");

            migrationBuilder.CreateIndex(
                name: "IX_FfTicketRegisters_TicketPriority",
                table: "FfTicketRegisters",
                column: "TicketPriority");

            migrationBuilder.CreateIndex(
                name: "IX_FfTicketRegisters_TicketShortCode",
                table: "FfTicketRegisters",
                column: "TicketShortCode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_FfTicketRegisters_TicketStatus",
                table: "FfTicketRegisters",
                column: "TicketStatus");

            migrationBuilder.CreateIndex(
                name: "IX_FfTicketRegisters_TicketTroubleType",
                table: "FfTicketRegisters",
                column: "TicketTroubleType");

            migrationBuilder.CreateIndex(
                name: "IX_FfUserProfiles_LocationCode",
                table: "FfUserProfiles",
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
                name: "FfExternalNotess");

            migrationBuilder.DropTable(
                name: "FfInternalNotess");

            migrationBuilder.DropTable(
                name: "FfTicketConstructoror");

            migrationBuilder.DropTable(
                name: "FfTicketFlows");

            migrationBuilder.DropTable(
                name: "AspNetRoles");

            migrationBuilder.DropTable(
                name: "FfTicketRegisters");

            migrationBuilder.DropTable(
                name: "FfPriorityCodess");

            migrationBuilder.DropTable(
                name: "FfStatusCodes");

            migrationBuilder.DropTable(
                name: "FfTicketTypess");

            migrationBuilder.DropTable(
                name: "FfUserProfiles");

            migrationBuilder.DropTable(
                name: "AspNetUsers");

            migrationBuilder.DropTable(
                name: "FfBuildingDirectorys");
        }
    }
}
