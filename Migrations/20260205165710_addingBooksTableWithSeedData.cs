using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace BookStoreAppSpring.Migrations
{
    /// <inheritdoc />
    public partial class addingBooksTableWithSeedData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Books",
                columns: table => new
                {
                    BookId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BookTitle = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Author = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Price = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ImgUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CategoryID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Books", x => x.BookId);
                    table.ForeignKey(
                        name: "FK_Books_Categories_CategoryID",
                        column: x => x.CategoryID,
                        principalTable: "Categories",
                        principalColumn: "CategoryId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Books",
                columns: new[] { "BookId", "Author", "BookTitle", "CategoryID", "Description", "ImgUrl", "Price" },
                values: new object[,]
                {
                    { 1, "J.K. Rowling", "Harry Potter and the Prisoner of Azkaban", 2, "When the Knight Bus crashes through the darkness and screeches to a halt in front of him, it's the start of another far from ordinary year at Hogwarts for Harry Potter.", "", 22.91m },
                    { 2, "DK Travel", "USA ANtional Parks: Land of Wonder", 1, "The USA's National Parks truly are places of wonder: staggering landscapes of jaw-dropping dimensions and incredible diversity where you can stand on the very edge of civilzation. They are the earth's breathing spaces, precious places to conserve nature and wildlife for future preservation.", "", 16.80m },
                    { 3, "Scott Selikoff", "OCP Oracle Certified Professional Java SE 17 Developer Study Guide: Exam 1Z0-829", 3, "In the OCP Oracle Certified Professional Java SE 17 Developer Study Guide: Exam 1Z0-829, you'll find accessible and essential test prep material for the in-demand and practical OCP Java SE 17 Developer certification.", "", 36.99m }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Books_CategoryID",
                table: "Books",
                column: "CategoryID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Books");
        }
    }
}
