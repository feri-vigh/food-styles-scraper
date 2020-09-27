using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace FoodStylesScraper.Data.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ScrapedMenuItems",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    MenuTitle = table.Column<string>(nullable: true),
                    MenuDescription = table.Column<string>(nullable: true),
                    MenuSectionTitle = table.Column<string>(nullable: true),
                    DishName = table.Column<string>(nullable: true),
                    DishDescription = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ScrapedMenuItems", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ScrapedMenuItems");
        }
    }
}
