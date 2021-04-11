using System.Linq;

using FluentAssertions;

using Npgsql;

using Xunit;

namespace Tests.Infrastructure.Persistence
{
    public abstract class IntegerKeyRepositoryTestsBase<
            TRepository,
            TResource>
        : RecipeResourceRepositoryTestBase<
            TRepository,
            TResource,
            int?>
        where TResource : class, new()
        where TRepository : class
    {
        protected IntegerKeyRepositoryTestsBase(DatabaseFixture fixture) : base(fixture)
        {
        }

        protected override int? GetKey(dynamic resource) => resource.Id;

        protected override int? MockKey() => 1;

        protected override int?[] MockKeys(int count) =>
            Enumerable.Range(0, count).Select(i => (int?) i).ToArray();

        [Fact]
        public void CreateOrUpdate_ThrowsException_WhenInvalidDataIsInput()
        {
            // Arrange
            string     unused          = Faker.Lorem.Sentence();
            TResource? invalidResource = MockResource(-1);
            
            // Act/Assert
            Repo.Invoking(async repo => await repo.CreateOrUpdateAsync(unused, invalidResource))
                .Should().Throw<NpgsqlException>();
        }
    }
}