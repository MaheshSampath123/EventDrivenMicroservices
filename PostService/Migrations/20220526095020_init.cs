using Microsoft.EntityFrameworkCore.Migrations;

namespace PostService.Migrations
{
    public partial class init : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "UserDetails",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(nullable: true),
                    Mail = table.Column<string>(nullable: true),
                    Password = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserDetails", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "PostDetails",
                columns: table => new
                {
                    PostId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(nullable: true),
                    Content = table.Column<string>(nullable: true),
                    UserId = table.Column<int>(nullable: false),
                    CategoryName = table.Column<string>(nullable: true),
                    BlobUrl = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PostDetails", x => x.PostId);
                    table.ForeignKey(
                        name: "FK_PostDetails_UserDetails_UserId",
                        column: x => x.UserId,
                        principalTable: "UserDetails",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PostDetails_UserId",
                table: "PostDetails",
                column: "UserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PostDetails");

            migrationBuilder.DropTable(
                name: "UserDetails");
        }
    }
}
