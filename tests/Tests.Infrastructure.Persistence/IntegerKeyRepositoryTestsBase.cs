using System.Linq;

using RecipeBook.Infrastructure.Persistence;

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

    }
}