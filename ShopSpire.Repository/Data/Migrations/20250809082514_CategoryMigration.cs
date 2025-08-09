using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ShopSpire.Repository.Data.Migrations
{
    public partial class CategoryMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "2beee72a-cbc3-4bab-8250-5d70a6b1a63d");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "c4758578-eb02-412c-bdf6-00d22c78afe2");

            migrationBuilder.CreateTable(
                name: "Categories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Categories", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "8525a01c-362a-4c46-aff6-cd757ee0ef9d", "79c33faa-0f67-430f-bfbe-24495c7a7d62", "User", "USER" });

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "f0bb8018-3f25-49bb-a5b2-0739d3895cb0", "9e4f4380-8152-4a1c-a714-69265a90fe12", "Admin", "ADMIN" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Categories");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "8525a01c-362a-4c46-aff6-cd757ee0ef9d");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "f0bb8018-3f25-49bb-a5b2-0739d3895cb0");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "2beee72a-cbc3-4bab-8250-5d70a6b1a63d", "935e73b3-09d1-4d62-9c8b-55451156c0c7", "User", "USER" });

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "c4758578-eb02-412c-bdf6-00d22c78afe2", "db5a5c16-c107-4e3c-a1a2-cfc8410e50c5", "Admin", "ADMIN" });
        }
    }
}
