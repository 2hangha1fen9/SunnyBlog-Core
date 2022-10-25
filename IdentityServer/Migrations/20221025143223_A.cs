using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IdentityService.Migrations
{
    public partial class A : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Permission",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    service = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    controller = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    action = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    description = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    updateTime = table.Column<DateTime>(type: "datetime", nullable: true),
                    createTime = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(now())"),
                    status = table.Column<int>(type: "int", nullable: false, defaultValueSql: "((1))", comment: "1启用-1禁用"),
                    isPublic = table.Column<int>(type: "int", nullable: false, defaultValueSql: "((-1))", comment: "1匿名权限-1私有权限")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Permission", x => x.id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Role",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    name = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    status = table.Column<int>(type: "int", nullable: false, defaultValueSql: "((1))", comment: "-1禁用1启用"),
                    createTime = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(now())"),
                    updateTime = table.Column<DateTime>(type: "datetime", nullable: true),
                    isDefault = table.Column<int>(type: "int", nullable: false, comment: "1默认-1不默认")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Role", x => x.id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "RolePermissionRelation",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    roleId = table.Column<int>(type: "int", nullable: false),
                    permissionId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RolePermissionRelation", x => x.id);
                    table.ForeignKey(
                        name: "FK_RolePermissionRelation_Permission",
                        column: x => x.permissionId,
                        principalTable: "Permission",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RolePermissionRelation_Role",
                        column: x => x.roleId,
                        principalTable: "Role",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "UserRoleRelation",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    userId = table.Column<int>(type: "int", nullable: false),
                    roleId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserRoleRelation", x => x.id);
                    table.ForeignKey(
                        name: "FK_UserRoleRelation_Role",
                        column: x => x.roleId,
                        principalTable: "Role",
                        principalColumn: "id");
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_Permission",
                table: "Permission",
                columns: new[] { "action", "controller", "service" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Role",
                table: "Role",
                column: "name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RolePermissionRelation",
                table: "RolePermissionRelation",
                columns: new[] { "permissionId", "roleId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RolePermissionRelation_roleId",
                table: "RolePermissionRelation",
                column: "roleId");

            migrationBuilder.CreateIndex(
                name: "IX_UserRoleRelation",
                table: "UserRoleRelation",
                columns: new[] { "roleId", "userId" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RolePermissionRelation");

            migrationBuilder.DropTable(
                name: "UserRoleRelation");

            migrationBuilder.DropTable(
                name: "Permission");

            migrationBuilder.DropTable(
                name: "Role");
        }
    }
}
