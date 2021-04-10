using RecipeBook.Core.Domain.Recipes;
using RecipeBook.Infrastructure.Persistence;

namespace Tests.Infrastructure.Persistence
{
    public class UsedOccasionsRepositoryTestsTests 
        : IntegerKeyRepositoryTestsBase<UsedOccasionsRepository, UsedOccasion>
    {
        public UsedOccasionsRepositoryTestsTests(DatabaseFixture fixture) : base(fixture)
        {
            Repo = new UsedOccasionsRepository(MockLogger.Object, fixture.ConnectionString);
        }

        protected override string InsertResourceSql(int recipeId) => $@"
            INSERT INTO usedoccasions (id, comment, date, duration, recipeid)
                VALUES (:Id, :Comment, :Date, :Duration, {recipeId})  RETURNING *;
        ";
        protected override string ResourceExistsSql =>
            "SELECT EXISTS(SELECT 1 FROM usedoccasions WHERE id = :key)";
        
        protected override UsedOccasion MockResource(int? key = default)
        {
            return new()
            {
                Id = key ?? MockKey(),
                Comment = Faker.Lorem.Sentence(),
                Date = Faker.Date.Recent(),
                Duration = Faker.Date.Timespan()
            };
        }
    }
}