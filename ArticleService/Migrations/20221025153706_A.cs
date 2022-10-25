using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ArticleService.Migrations
{
    public partial class A : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "ArtCategory",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    name = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    userId = table.Column<int>(type: "int", nullable: false),
                    parentId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ArtCategory", x => x.id);
                    table.ForeignKey(
                        name: "FK_Category_Category",
                        column: x => x.parentId,
                        principalTable: "ArtCategory",
                        principalColumn: "id");
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "ArtRegion",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    name = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    description = table.Column<string>(type: "text", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    parentId = table.Column<int>(type: "int", nullable: true),
                    status = table.Column<int>(type: "int", nullable: false, defaultValueSql: "((1))", comment: "1启用-1禁用")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ArtRegion", x => x.id);
                    table.ForeignKey(
                        name: "FK_ArtRegion_ArtRegion",
                        column: x => x.parentId,
                        principalTable: "ArtRegion",
                        principalColumn: "id");
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "GlobalSetting",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Option = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Value = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GlobalSetting", x => x.id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Tags",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    name = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    userId = table.Column<int>(type: "int", nullable: false),
                    color = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    isPrivate = table.Column<int>(type: "int", nullable: false, defaultValueSql: "((-1))", comment: "-1私有标签1公共标签")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tags", x => x.id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Article",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    title = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    content = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    codeStyle = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    contentStyle = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    summary = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    photo = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    userId = table.Column<int>(type: "int", nullable: false),
                    regionId = table.Column<int>(type: "int", nullable: true),
                    categoryId = table.Column<int>(type: "int", nullable: true),
                    status = table.Column<int>(type: "int", nullable: false, defaultValueSql: "((1))", comment: "-1待审核1已发布2私有3回收站4草稿"),
                    commentStatus = table.Column<int>(type: "int", nullable: false, defaultValueSql: "((1))", comment: "-1不可以评论1可评论2需要审核评论"),
                    createTime = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(now())"),
                    updateTime = table.Column<DateTime>(type: "datetime", nullable: true),
                    isLock = table.Column<int>(type: "int", nullable: false, defaultValueSql: "((1))", comment: "1未锁定-1锁定")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Article", x => x.id);
                    table.ForeignKey(
                        name: "FK_Article_ArtRegion",
                        column: x => x.regionId,
                        principalTable: "ArtRegion",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Article_Category",
                        column: x => x.categoryId,
                        principalTable: "ArtCategory",
                        principalColumn: "id");
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "ArticleTag",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    articleId = table.Column<int>(type: "int", nullable: false),
                    tagId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ArticleTag", x => x.id);
                    table.ForeignKey(
                        name: "FK_ArticleTag_Article",
                        column: x => x.articleId,
                        principalTable: "Article",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ArticleTag_Tags",
                        column: x => x.tagId,
                        principalTable: "Tags",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_ArtCategory",
                table: "ArtCategory",
                columns: new[] { "name", "parentId", "userId" });

            migrationBuilder.CreateIndex(
                name: "IX_ArtCategory_parentId",
                table: "ArtCategory",
                column: "parentId");

            migrationBuilder.CreateIndex(
                name: "IX_Article",
                table: "Article",
                column: "createTime");

            migrationBuilder.CreateIndex(
                name: "IX_Article_categoryId",
                table: "Article",
                column: "categoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Article_regionId",
                table: "Article",
                column: "regionId");

            migrationBuilder.CreateIndex(
                name: "IX_ArticleTag",
                table: "ArticleTag",
                columns: new[] { "articleId", "tagId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ArticleTag_tagId",
                table: "ArticleTag",
                column: "tagId");

            migrationBuilder.CreateIndex(
                name: "IX_ArtRegion",
                table: "ArtRegion",
                columns: new[] { "id", "parentId" });

            migrationBuilder.CreateIndex(
                name: "IX_ArtRegion_1",
                table: "ArtRegion",
                column: "name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ArtRegion_parentId",
                table: "ArtRegion",
                column: "parentId");

            migrationBuilder.CreateIndex(
                name: "IX_Tags",
                table: "Tags",
                column: "name",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ArticleTag");

            migrationBuilder.DropTable(
                name: "GlobalSetting");

            migrationBuilder.DropTable(
                name: "Article");

            migrationBuilder.DropTable(
                name: "Tags");

            migrationBuilder.DropTable(
                name: "ArtRegion");

            migrationBuilder.DropTable(
                name: "ArtCategory");
        }
    }
}
