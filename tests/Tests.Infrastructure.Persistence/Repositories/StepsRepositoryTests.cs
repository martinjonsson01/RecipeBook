using RecipeBook.Core.Domain.Recipes;
using RecipeBook.Infrastructure.Persistence.Repositories;

namespace Tests.Infrastructure.Persistence.Repositories
{
    public class StepsRepositoryTests : IntegerKeyRepositoryTestsBase<StepsRepository, Step>
    {
        public StepsRepositoryTests(DatabaseFixture fixture) : base(fixture)
        {
            Repo = new StepsRepository(MockLogger.Object, fixture.ConnectionString);
        }

        protected override string InsertResourceSql(int recipeId) => $@"
            INSERT INTO steps (id, number, instruction, recipeid)
                VALUES (:Id, :Number, :Instruction, {recipeId})  RETURNING *;
        ";

        protected override string ResourceExistsSql =>
            "SELECT EXISTS(SELECT 1 FROM steps WHERE id = :key)";

        protected override Step MockResource(int? key = default)
        {
            return new()
            {
                Id = key ?? MockKey(),
                Number = Faker.Random.Int(1, 10),
                Instruction = Faker.Lorem.Sentences(2)
            };
        }
    }
}