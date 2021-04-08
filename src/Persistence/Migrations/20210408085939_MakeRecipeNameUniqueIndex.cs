using Microsoft.EntityFrameworkCore.Migrations;

namespace RecipeBook.Infrastructure.Persistence.Migrations
{
    public partial class MakeRecipeNameUniqueIndex : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Recipes_Name",
                table: "Recipes",
                column: "Name",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Recipes_Name",
                table: "Recipes");
        }
    }
}
