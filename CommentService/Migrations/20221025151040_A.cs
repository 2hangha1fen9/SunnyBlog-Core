using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CommentService.Migrations
{
    public partial class A : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Comments",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    content = table.Column<string>(type: "text", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    userId = table.Column<int>(type: "int", nullable: false),
                    articleId = table.Column<int>(type: "int", nullable: false),
                    createTime = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(now())"),
                    status = table.Column<int>(type: "int", nullable: false, defaultValueSql: "((1))", comment: "1审核通过2需要审核-1禁止评论"),
                    parentId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Comments", x => x.id);
                    table.ForeignKey(
                        name: "FK_Comments_Comments",
                        column: x => x.parentId,
                        principalTable: "Comments",
                        principalColumn: "id");
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Likes",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    articleId = table.Column<int>(type: "int", nullable: false),
                    userId = table.Column<int>(type: "int", nullable: false),
                    status = table.Column<int>(type: "int", nullable: false, defaultValueSql: "((1))", comment: "1点赞2收藏 3点赞又收藏")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Likes", x => x.id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Views",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    articleId = table.Column<int>(type: "int", nullable: false),
                    userId = table.Column<int>(type: "int", nullable: true, defaultValueSql: "((0))"),
                    ip = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    viewTime = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(now())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Views", x => x.id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_Comments_parentId",
                table: "Comments",
                column: "parentId");

            migrationBuilder.CreateIndex(
                name: "IX_ArtLike",
                table: "Likes",
                columns: new[] { "articleId", "userId", "status" },
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Comments");

            migrationBuilder.DropTable(
                name: "Likes");

            migrationBuilder.DropTable(
                name: "Views");
        }
    }
}
