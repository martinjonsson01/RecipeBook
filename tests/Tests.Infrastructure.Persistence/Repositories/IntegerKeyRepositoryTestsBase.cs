using System.Linq;
using System.Threading.Tasks;

using Dapper;

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

        protected override async Task<int?> MockKey() => await Db.QuerySingleAsync<int>($@"
            SELECT nextval('public.{typeof(TResource).Name.ToLowerInvariant()}s_id_seq');
        ");

        [Fact]
        public async void CreateOrUpdate_ThrowsException_WhenInvalidDataIsInput()
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