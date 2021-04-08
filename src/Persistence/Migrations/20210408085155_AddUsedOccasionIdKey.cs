using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace RecipeBook.Infrastructure.Persistence.Migrations
{
    public partial class AddUsedOccasionIdKey : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_UsedOccasions",
                table: "UsedOccasions");

            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "UsedOccasions",
                type: "integer",
                nullable: false,
                defaultValue: 0)
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AddPrimaryKey(
                name: "PK_UsedOccasions",
                table: "UsedOccasions",
                column: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_UsedOccasions",
                table: "UsedOccasions");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "UsedOccasions");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UsedOccasions",
                table: "UsedOccasions",
                column: "When");
        }
    }
}
