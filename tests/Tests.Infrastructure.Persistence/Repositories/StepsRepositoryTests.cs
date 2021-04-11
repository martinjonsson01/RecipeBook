using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Dapper;

using FluentAssertions;

using RecipeBook.Core.Domain.Recipes;
using RecipeBook.Infrastructure.Persistence.Repositories;

using Xunit;

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
        
        private static string InsertTimeStepSql => @"
            INSERT INTO timesteps (id, duration)
                VALUES (:Id, :Duration)  RETURNING *;
        ";

        protected override string ResourceExistsSql =>
            "SELECT EXISTS(SELECT 1 FROM steps WHERE id = :key)";

        protected override async Task<Step> MockResource(int? key = default)
        {
            return new()
            {
                Id = key ?? await MockKey(),
                Number = Faker.Random.Int(1, 10),
                Instruction = Faker.Lorem.Sentences(2)
            };
        }

        private TimeStep TimeStepFromStep(Step step)
        {
            return new()
            {
                Id = step.Id,
                Number = step.Number,
                Instruction = step.Instruction,
                Duration = Faker.Date.Timespan()
            };
        }

        private async Task<TimeStep> MockTimeStepInDatabaseAsync(string recipeName, int? key = null)
        {
            Step     mockedParentStep     = await MockResourceInDatabaseAsync(recipeName, key);
            TimeStep mockedTimeStep = TimeStepFromStep(mockedParentStep);

            return await StoreTimeStepInDatabase(mockedTimeStep);
        }

        private async Task<TimeStep> StoreTimeStepInDatabase(TimeStep timeStep)
        {
            return await Db.QuerySingleAsync<TimeStep>(InsertTimeStepSql, timeStep);
        }

        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        public async void GetAll_IncludesTimeSteps_WhenNResourcesContainingOneTimeStepExist(int n)
        {
            // Arrange
            string     recipeName       = Faker.Lorem.Sentence();
            List<Step> resources        = (await MockResourcesInDatabase(recipeName, n)).ToList();
            TimeStep   expectedTimeStep = await MockTimeStepInDatabaseAsync(recipeName);
            resources.Add(expectedTimeStep);

            // Act
            Step[] actualResources = (await Repo.GetAllAsync(recipeName)).ToArray();

            // Assert
            actualResources.Should().ContainEquivalentOf(expectedTimeStep);
            actualResources.Should().BeEquivalentTo(resources);
        }
        
        [Fact]
        public async void Get_ReturnsTimeStep_WhenTimeStepExists()
        {
            // Arrange
            string    recipeName       = Faker.Lorem.Sentence();
            TimeStep expectedResource = await MockTimeStepInDatabaseAsync(recipeName);
            int?     expectedKey      = GetKey(expectedResource);

            // Act
            Step? actualResource = await Repo.GetAsync(recipeName, expectedKey);

            // Assert
            actualResource.Should().BeAssignableTo<TimeStep>();
            actualResource.Should().BeEquivalentTo(expectedResource);
        }
    }
}