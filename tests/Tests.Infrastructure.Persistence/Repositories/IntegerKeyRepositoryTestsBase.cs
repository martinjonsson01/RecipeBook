using System.Threading.Tasks;

using FluentAssertions;

using Npgsql;

using Xunit;

namespace Tests.Infrastructure.Persistence.Repositories
{
    public abstract class IntegerKeyRepositoryTestsBase<
            TRepository,
            TResource>
        : RepositoryTestBase<
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

        [Fact]
        public async Task CreateOrUpdate_ThrowsException_WhenInvalidDataIsInput()
        {
            // Arrange
            string     unused          = Faker.Lorem.Sentence();
            TResource? invalidResource = await MockResource(-1);
            
            // Act/Assert
            Repo.Invoking(async repo => await repo.CreateOrUpdateAsync(unused, invalidResource))
                .Should().Throw<NpgsqlException>();
        }
    }
}