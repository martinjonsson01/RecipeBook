using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

using Bogus;

using Dapper;

using FluentAssertions;

using Microsoft.Extensions.Logging;

using Moq;

using RecipeBook.Infrastructure.Persistence;

using Xunit;

namespace Tests.Infrastructure.Persistence
{
    [Collection("RepositoryTests")]
    public abstract class RecipeResourceRepositoryTestBase<
        TRecipeResourceRepository,
        TRepository,
        TResource,
        TKey> : IClassFixture<DatabaseFixture>
        where TRecipeResourceRepository : RecipeResourceRepository<TRepository, TResource, TKey>
        where TResource : class, new()
        where TRepository : class
    {
        protected RecipeResourceRepositoryTestBase(DatabaseFixture fixture)
        {
            MockLogger = new Mock<ILogger<TRepository>>();
            Fixture = fixture;
            Db = fixture.Db;
            Faker = new Faker("sv");

            DatabaseFixture.Checkpoint.Reset(fixture.Db).Wait();
        }

        protected          TRecipeResourceRepository  Repo = null!; // Has to be set by subtype constructor
        protected readonly Mock<ILogger<TRepository>> MockLogger;
        protected readonly DatabaseFixture            Fixture;
        protected readonly IDbConnection              Db;
        protected readonly Faker                      Faker;


        protected abstract TKey   GetKey(TResource resource);
        protected abstract TKey   MockKey();
        protected abstract TKey[] MockKeys(int count);

        protected virtual string InsertOrGetParentRecipeSql => @"
            INSERT INTO recipes (id, name) VALUES (DEFAULT, :recipeName)
                ON CONFLICT (name) DO UPDATE SET name = :recipeName 
                RETURNING id;
        ";

        protected abstract string InsertResourceSql(int recipeId);
        protected abstract string ResourceExistsSql { get; }

        protected abstract TResource MockResource(TKey key = default);

        protected async Task<TResource> MockResourceInDatabaseAsync(string recipeName, TKey key = default)
        {
            if (key == null || key.Equals(default))
            {
                key = MockKey();
            }
            TResource mockedResource = MockResource(key);

            return await StoreInDatabase(recipeName, mockedResource);
        }

        protected async Task<TResource> StoreInDatabase(string recipeName, TResource mockedResource)
        {
            var recipeId = await Db.QuerySingleAsync<int>(InsertOrGetParentRecipeSql, new { recipeName });
            return await Db.QuerySingleAsync<TResource>(InsertResourceSql(recipeId), mockedResource);
        }

        protected async Task<IEnumerable<TResource>> MockResourcesInDatabase(string recipeName, int resourcesCount)
        {
            TKey[] keys = MockKeys(resourcesCount);
            return await MockResourcesInDatabase(recipeName, keys);
        }

        protected async Task<IEnumerable<TResource>> MockResourcesInDatabase(
            string        recipeName,
            params TKey[] ids)
        {
            var mockedResources = new List<TResource>();
            foreach (TKey key in ids)
            {
                mockedResources.Add(await MockResourceInDatabaseAsync(recipeName, key));
            }

            return mockedResources;
        }

        protected async Task<bool> ExistsInDatabase(TKey key)
        {
            var existsInDatabase = await Db.QuerySingleAsync<bool>(ResourceExistsSql, new { key });
            return existsInDatabase;
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(2)]
        public async void GetAll_ReturnsAll_WhenNResourcesExist(int n)
        {
            // Arrange
            string                 recipeName        = Faker.Lorem.Sentence();
            IEnumerable<TResource> expectedResources = await MockResourcesInDatabase(recipeName, n);

            // Act
            IEnumerable<TResource> actualResources = await Repo.GetAllAsync(recipeName);

            // Assert
            actualResources.Should().BeEquivalentTo(expectedResources);
        }

        [Fact]
        public async void Get_ReturnsResource_WhenResourceExists()
        {
            // Arrange
            string    recipeName       = Faker.Lorem.Sentence();
            TResource expectedResource = await MockResourceInDatabaseAsync(recipeName);
            TKey      expectedKey      = GetKey(expectedResource);

            // Act
            TResource? actualResource = await Repo.GetAsync(recipeName, expectedKey);

            // Assert
            actualResource.Should().NotBeNull();
            actualResource.Should().BeEquivalentTo(expectedResource);
        }

        [Fact]
        public async void Get_ReturnsNull_WhenNoResourceExists()
        {
            // Arrange
            string    recipeName       = Faker.Lorem.Sentence();
            TResource expectedResource = MockResource();
            TKey      expectedKey      = GetKey(expectedResource);

            // Act
            TResource? actualResource = await Repo.GetAsync(recipeName, expectedKey);

            // Assert
            actualResource.Should().BeNull();
        }

        [Fact]
        public async void Exists_ReturnsTrue_WhenResourceExists()
        {
            // Arrange
            string    recipeName       = Faker.Lorem.Sentence();
            TResource expectedResource = await MockResourceInDatabaseAsync(recipeName);
            TKey      expectedKey      = GetKey(expectedResource);

            // Act
            bool exists = await Repo.ExistsAsync(recipeName, expectedKey);

            // Assert
            exists.Should().BeTrue();
        }

        [Fact]
        public async void Exists_ReturnsFalse_WhenNoResourceExists()
        {
            // Arrange
            string    recipeName       = Faker.Lorem.Sentence();
            TResource expectedResource = MockResource();
            TKey      expectedKey      = GetKey(expectedResource);

            // Act
            bool exists = await Repo.ExistsAsync(recipeName, expectedKey);

            // Assert
            exists.Should().BeFalse();
        }

        [Fact]
        public async void CreateOrUpdate_CreatesNewResource_WhenNoResourceExists()
        {
            // Arrange
            string recipeName = Faker.Lorem.Sentence();
            // Mock unused resource so that parent recipe is created.
            await MockResourceInDatabaseAsync(recipeName);

            TResource expectedResource = MockResource();

            // Act
            TResource? actualResource = await Repo.CreateOrUpdateAsync(recipeName, expectedResource);

            // Assert
            actualResource.Should().NotBeNull();
            TKey key = GetKey(actualResource!);
            (await ExistsInDatabase(key)).Should().BeTrue();
        }

        [Fact]
        public async void CreateOrUpdate_ReturnsResource_WhenResourceExists()
        {
            // Arrange
            string recipeName = Faker.Lorem.Sentence();
            TResource expectedResource = await MockResourceInDatabaseAsync(recipeName);

            // Act
            TResource? actualResource = await Repo.CreateOrUpdateAsync(recipeName, expectedResource);

            // Assert
            actualResource.Should().BeEquivalentTo(expectedResource);
        }

        [Fact]
        public async void Delete_DeletesResource_WhenResourceExists()
        {
            // Arrange
            string    recipeName       = Faker.Lorem.Sentence();
            TResource expectedResource = await MockResourceInDatabaseAsync(recipeName);
            TKey      expectedKey      = GetKey(expectedResource);

            // Assert
            (await ExistsInDatabase(expectedKey)).Should().BeTrue();
            // Act 
            await Repo.DeleteAsync(recipeName, expectedKey);
            // Assert
            (await ExistsInDatabase(expectedKey)).Should().BeFalse();
        }

        [Fact]
        public async void Delete_DoesNothing_WhenNoResourceExists()
        {
            // Arrange
            string    recipeName       = Faker.Lorem.Sentence();
            TResource expectedResource = MockResource();
            TKey      expectedKey      = GetKey(expectedResource);

            // Assert
            (await ExistsInDatabase(expectedKey)).Should().BeFalse();
            // Act
            await Repo.DeleteAsync(recipeName, expectedKey);
            // Assert
            (await ExistsInDatabase(expectedKey)).Should().BeFalse();
        }
    }
}