using System.Threading.Tasks;

using RecipeBook.Core.Domain.Recipes;
using RecipeBook.Infrastructure.Persistence.Repositories;

namespace Tests.Infrastructure.Persistence.Repositories
{
    public class UsedOccasionsRepositoryTests 
        : IntegerKeyRepositoryTestsBase<UsedOccasionsRepository, UsedOccasion>
    {
        public UsedOccasionsRepositoryTests(DatabaseFixture fixture) : base(fixture)
        {
            Repo = new UsedOccasionsRepository(MockLogger.Object, fixture.ConnectionString);
        }

        protected override string InsertResourceSql(int recipeId) => $@"
            INSERT INTO usedoccasions (id, comment, date, duration, recipeid)
                VALUES (:Id, :Comment, :Date, :Duration, {recipeId})  RETURNING *;
        ";
        protected override string ResourceExistsSql =>
            "SELECT EXISTS(SELECT 1 FROM usedoccasions WHERE id = :key)";
        
        protected override async Task<UsedOccasion> MockResource(int? key = default)
        {
            return new()
            {
                Id = key ?? await MockKey(),
                Comment = Faker.Lorem.Sentence(),
                Date = Faker.Date.Recent(),
                Duration = Faker.Date.Timespan()
            };
        }
    }
}