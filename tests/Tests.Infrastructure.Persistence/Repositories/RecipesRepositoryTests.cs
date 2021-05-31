using System.Linq;
using System.Threading.Tasks;

using FluentAssertions;

using Moq;

using RecipeBook.Core.Application.Repositories;
using RecipeBook.Core.Domain.Recipes;
using RecipeBook.Infrastructure.Persistence.Repositories;

using Tests.Shared;

using Xunit;

namespace Tests.Infrastructure.Persistence.Repositories
{
    public class RecipesRepositoryTests
        : RepositoryTestBase<
            RecipesRepository,
            Recipe,
            string>
    {
        public RecipesRepositoryTests(DatabaseFixture fixture) : base(fixture.Db)
        {
            var mockStepRepo       = new Mock<IResourcesRepository<Step, int?>>();
            var mockIngredientRepo = new Mock<IResourcesRepository<Ingredient, int?>>();
            Repo = new RecipesRepository(MockLogger.Object, mockStepRepo.Object, mockIngredientRepo.Object, fixture.ConnectionString);
        }

        protected override string GetKey(dynamic resource) => resource.Name;

        protected override Task<string> MockKey() => Task.FromResult(Faker.Lorem.Sentence());

        protected override string[] MockKeys(int count) =>
            Enumerable.Range(0, count).Select(_ => MockKey().Result).ToArray();

        protected override string InsertOrGetParentRecipeSql => "SELECT 1;"; // no need for empty recipes

        protected override string InsertResourceSql(int recipeId) => @"
            INSERT INTO recipes VALUES (DEFAULT, :Rating, :ImagePath, :Name) RETURNING *;
        ";

        protected override string ResourceExistsSql =>
            "SELECT EXISTS(SELECT 1 FROM recipes WHERE name = :key)";

        protected override Task<Recipe> MockResource(string? key = default)
        {
            return Task.FromResult(Fakers.Recipe.Generate());
        }

        [Fact]
        public async Task CreateOrUpdate_ReturnsNull_WhenRecipeWithSameNameButDifferentId()
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