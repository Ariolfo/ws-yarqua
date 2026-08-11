using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Yarqua.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class IdentityMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "YarqtbRol",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_YarqtbRol", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "YarqtbUsuario",
                columns: table => new
                {
                    Usua_Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Usua_Nombre = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Ciu_Id = table.Column<int>(type: "int", nullable: true),
                    Usua_FechaRegistro = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Usua_FechaCreacion = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Usua_FechaActualizacion = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Usua_Activo = table.Column<bool>(type: "bit", nullable: false),
                    UserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedUserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedEmail = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SecurityStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    TwoFactorEnabled = table.Column<bool>(type: "bit", nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    LockoutEnabled = table.Column<bool>(type: "bit", nullable: false),
                    AccessFailedCount = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_YarqtbUsuario", x => x.Usua_Id);
                    table.ForeignKey(
                        name: "FK_YarqtbUsuario_YarqtbCiudad_Ciu_Id",
                        column: x => x.Ciu_Id,
                        principalTable: "YarqtbCiudad",
                        principalColumn: "Ciu_Id");
                });

            migrationBuilder.CreateTable(
                name: "YarqtbRolClaim",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoleId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_YarqtbRolClaim", x => x.Id);
                    table.ForeignKey(
                        name: "FK_YarqtbRolClaim_YarqtbRol_RoleId",
                        column: x => x.RoleId,
                        principalTable: "YarqtbRol",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "YarqtbUsuarioClaim",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_YarqtbUsuarioClaim", x => x.Id);
                    table.ForeignKey(
                        name: "FK_YarqtbUsuarioClaim_YarqtbUsuario_UserId",
                        column: x => x.UserId,
                        principalTable: "YarqtbUsuario",
                        principalColumn: "Usua_Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "YarqtbUsuarioLogin",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProviderKey = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProviderDisplayName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_YarqtbUsuarioLogin", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_YarqtbUsuarioLogin_YarqtbUsuario_UserId",
                        column: x => x.UserId,
                        principalTable: "YarqtbUsuario",
                        principalColumn: "Usua_Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "YarqtbUsuarioRol",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    RoleId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_YarqtbUsuarioRol", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_YarqtbUsuarioRol_YarqtbRol_RoleId",
                        column: x => x.RoleId,
                        principalTable: "YarqtbRol",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_YarqtbUsuarioRol_YarqtbUsuario_UserId",
                        column: x => x.UserId,
                        principalTable: "YarqtbUsuario",
                        principalColumn: "Usua_Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "YarqtbUsuarioToken",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    LoginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_YarqtbUsuarioToken", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_YarqtbUsuarioToken_YarqtbUsuario_UserId",
                        column: x => x.UserId,
                        principalTable: "YarqtbUsuario",
                        principalColumn: "Usua_Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "YarqtbRol",
                column: "NormalizedName",
                unique: true,
                filter: "[NormalizedName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_YarqtbRolClaim_RoleId",
                table: "YarqtbRolClaim",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "YarqtbUsuario",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "IX_YarqtbUsuario_Ciu_Id",
                table: "YarqtbUsuario",
                column: "Ciu_Id");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "YarqtbUsuario",
                column: "NormalizedUserName",
                unique: true,
                filter: "[NormalizedUserName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_YarqtbUsuarioClaim_UserId",
                table: "YarqtbUsuarioClaim",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_YarqtbUsuarioLogin_UserId",
                table: "YarqtbUsuarioLogin",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_YarqtbUsuarioRol_RoleId",
                table: "YarqtbUsuarioRol",
                column: "RoleId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "YarqtbRolClaim");

            migrationBuilder.DropTable(
                name: "YarqtbUsuarioClaim");

            migrationBuilder.DropTable(
                name: "YarqtbUsuarioLogin");

            migrationBuilder.DropTable(
                name: "YarqtbUsuarioRol");

            migrationBuilder.DropTable(
                name: "YarqtbUsuarioToken");

            migrationBuilder.DropTable(
                name: "YarqtbRol");

            migrationBuilder.DropTable(
                name: "YarqtbUsuario");
        }
    }
}
