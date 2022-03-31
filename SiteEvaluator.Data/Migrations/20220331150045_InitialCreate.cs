using Microsoft.EntityFrameworkCore.Migrations;

namespace SiteEvaluator.Data.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PageInfos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SourceHost = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ScannerType = table.Column<int>(type: "int", nullable: false),
                    Url = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Content = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TotalLoadTime = table.Column<long>(type: "bigint", nullable: false),
                    Level = table.Column<int>(type: "int", nullable: false),
                    TotalSize = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PageInfos", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PageInfoUrls",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Url = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PageInfoUrlType = table.Column<int>(type: "int", nullable: false),
                    PageInfoId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PageInfoUrls", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PageInfoUrls_PageInfos_PageInfoId",
                        column: x => x.PageInfoId,
                        principalTable: "PageInfos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PageInfoUrls_PageInfoId",
                table: "PageInfoUrls",
                column: "PageInfoId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PageInfoUrls");

            migrationBuilder.DropTable(
                name: "PageInfos");
        }
    }
}
