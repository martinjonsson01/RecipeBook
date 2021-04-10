using System.Linq;

using FluentAssertions;

using RecipeBook.Core.Domain.Recipes;
using RecipeBook.Infrastructure.Persistence;

using Xunit;

namespace Tests.Infrastructure.Persistence
{
    public class RecipesRepositoryTests
        : RecipeResourceRepositoryTestBase<
            RecipeResourceRepository<RecipesRepository, Recipe, string>,
            RecipesRepository,
            Recipe,
            string>
    {
        public RecipesRepositoryTests(DatabaseFixture fixture) : base(fixture)
        {
            Repo = new RecipesRepository(MockLogger.Object, fixture.ConnectionString);
        }

        protected override string GetKey(dynamic resource) => resource.Name;

        protected override string MockKey() => Faker.Lorem.Sentence();

        protected override string[] MockKeys(int count) =>
            Enumerable.Range(0, count).Select(_ => MockKey()).ToArray();

        protected override string InsertOrGetParentRecipeSql => "SELECT 1;"; // no need for empty recipes
        protected override string InsertResourceSql(int recipeId) => @"
            INSERT INTO recipes VALUES (DEFAULT, :Rating, :ImagePath, :Name) RETURNING *;
        ";
        protected override string ResourceExistsSql => 
            "SELECT EXISTS(SELECT 1 FROM recipes WHERE name = :key)";

        protected override Recipe MockResource(string? key = default)
        {
            return new()
            {
                Name = key ?? MockKey(),
            };
        }

        [Fact]
        public async void CreateOrUpdate_ReturnsNull_WhenRecipeWithSameNameButDifferentId()
        {
            // Arrange
            string unused         = Faker.Lorem.Sentence();
            Recipe existingRecipe = await MockResourceInDatabaseAsync(unused);
            var conflictingRecipe = new Recipe
            {
                Id = existingRecipe.Id + 1,
                Name = existingRecipe.Name
            };

            // Act
            Recipe? actualRecipe = await Repo.CreateOrUpdateAsync(unused, conflictingRecipe);

            // Assert
            actualRecipe.Should().BeNull();
        }
    }
}