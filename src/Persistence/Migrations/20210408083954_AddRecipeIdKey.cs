using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace RecipeBook.Infrastructure.Persistence.Migrations
{
    public partial class AddRecipeIdKey : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Ingredients_Recipes_RecipeName",
                table: "Ingredients");

            migrationBuilder.DropForeignKey(
                name: "FK_Steps_Recipes_RecipeName",
                table: "Steps");

            migrationBuilder.DropForeignKey(
                name: "FK_UsedOccasions_Recipes_RecipeName",
                table: "UsedOccasions");

            migrationBuilder.DropIndex(
                name: "IX_UsedOccasions_RecipeName",
                table: "UsedOccasions");

            migrationBuilder.DropIndex(
                name: "IX_Steps_RecipeName",
                table: "Steps");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Recipes",
                table: "Recipes");

            migrationBuilder.DropIndex(
                name: "IX_Ingredients_RecipeName",
                table: "Ingredients");

            migrationBuilder.DropColumn(
                name: "RecipeName",
                table: "UsedOccasions");

            migrationBuilder.DropColumn(
                name: "RecipeName",
                table: "Steps");

            migrationBuilder.DropColumn(
                name: "RecipeName",
                table: "Ingredients");

            migrationBuilder.AddColumn<int>(
                name: "RecipeId",
                table: "UsedOccasions",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "RecipeId",
                table: "Steps",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "Recipes",
                type: "integer",
                nullable: false,
                defaultValue: 0)
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AddColumn<int>(
                name: "RecipeId",
                table: "Ingredients",
                type: "integer",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Recipes",
                table: "Recipes",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_UsedOccasions_RecipeId",
                table: "UsedOccasions",
                column: "RecipeId");

            migrationBuilder.CreateIndex(
                name: "IX_Steps_RecipeId",
                table: "Steps",
                column: "RecipeId");

            migrationBuilder.CreateIndex(
                name: "IX_Ingredients_RecipeId",
                table: "Ingredients",
                column: "RecipeId");

            migrationBuilder.AddForeignKey(
                name: "FK_Ingredients_Recipes_RecipeId",
                table: "Ingredients",
                column: "RecipeId",
                principalTable: "Recipes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Steps_Recipes_RecipeId",
                table: "Steps",
                column: "RecipeId",
                principalTable: "Recipes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_UsedOccasions_Recipes_RecipeId",
                table: "UsedOccasions",
                column: "RecipeId",
                principalTable: "Recipes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Ingredients_Recipes_RecipeId",
                table: "Ingredients");

            migrationBuilder.DropForeignKey(
                name: "FK_Steps_Recipes_RecipeId",
                table: "Steps");

            migrationBuilder.DropForeignKey(
                name: "FK_UsedOccasions_Recipes_RecipeId",
                table: "UsedOccasions");

            migrationBuilder.DropIndex(
                name: "IX_UsedOccasions_RecipeId",
                table: "UsedOccasions");

            migrationBuilder.DropIndex(
                name: "IX_Steps_RecipeId",
                table: "Steps");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Recipes",
                table: "Recipes");

            migrationBuilder.DropIndex(
                name: "IX_Ingredients_RecipeId",
                table: "Ingredients");

            migrationBuilder.DropColumn(
                name: "RecipeId",
                table: "UsedOccasions");

            migrationBuilder.DropColumn(
                name: "RecipeId",
                table: "Steps");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "Recipes");

            migrationBuilder.DropColumn(
                name: "RecipeId",
                table: "Ingredients");

            migrationBuilder.AddColumn<string>(
                name: "RecipeName",
                table: "UsedOccasions",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RecipeName",
                table: "Steps",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RecipeName",
                table: "Ingredients",
                type: "text",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Recipes",
                table: "Recipes",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_UsedOccasions_RecipeName",
                table: "UsedOccasions",
                column: "RecipeName");

            migrationBuilder.CreateIndex(
                name: "IX_Steps_RecipeName",
                table: "Steps",
                column: "RecipeName");

            migrationBuilder.CreateIndex(
                name: "IX_Ingredients_RecipeName",
                table: "Ingredients",
                column: "RecipeName");

            migrationBuilder.AddForeignKey(
                name: "FK_Ingredients_Recipes_RecipeName",
                table: "Ingredients",
                column: "RecipeName",
                principalTable: "Recipes",
                principalColumn: "Name",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Steps_Recipes_RecipeName",
                table: "Steps",
                column: "RecipeName",
                principalTable: "Recipes",
                principalColumn: "Name",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_UsedOccasions_Recipes_RecipeName",
                table: "UsedOccasions",
                column: "RecipeName",
                principalTable: "Recipes",
                principalColumn: "Name",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
