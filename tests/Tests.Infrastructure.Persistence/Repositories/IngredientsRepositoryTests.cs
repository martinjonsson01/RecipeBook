using System.Threading.Tasks;

using RecipeBook.Core.Domain.Recipes;
using RecipeBook.Core.Domain.Units;
using RecipeBook.Infrastructure.Persistence.Repositories;

namespace Tests.Infrastructure.Persistence.Repositories
{
    public class IngredientsRepositoryTests 
        : IntegerKeyRepositoryTestsBase<IngredientsRepository, Ingredient>
    {
        public IngredientsRepositoryTests(DatabaseFixture fixture) : base(fixture)
        {
            Repo = new IngredientsRepository(MockLogger.Object, fixture.ConnectionString);
        }

        protected override string InsertResourceSql(int recipeId) => $@"
            INSERT INTO ingredients (id, name, recipeid)
                VALUES (:Id, :Name, {recipeId})  RETURNING *;
        ";

        protected override string ResourceExistsSql =>
            "SELECT EXISTS(SELECT 1 FROM ingredients WHERE id = :key)";

        protected override async Task<Ingredient> MockResource(int? key = default)
        {
            return new()
            {
                Id = key ?? await MockKey(),
                Name = Faker.Lorem.Sentence(),
                Amount = Faker.PickRandom<Unit>(
                    new Mass(Faker.Random.Double(10)), 
                    new Volume(Faker.Random.Double(10)
                ))
            };
        }
    }
}