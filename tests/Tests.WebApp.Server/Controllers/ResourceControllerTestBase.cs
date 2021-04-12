using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Bogus;

using FluentAssertions;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

using Moq;

using RecipeBook.Core.Application.Repository;
using RecipeBook.Presentation.WebApp.Server.Controllers.v1;

using Xunit;

namespace Tests.WebApp.Server.Controllers
{
    public abstract class ResourceControllerTestBase<
        TController,
        TResource,
        TKey>
        where TResource : class, new()
        where TController : class
    {
        protected ResourceControllerTestBase()
        {
            MockRepo = new Mock<IResourcesRepository<TResource, TKey>>();
            MockLogger = new Mock<ILogger<TController>>();
            Faker = new Faker("sv");
        }

        protected          ResourceController<TController, TResource, TKey> Controller = null!; // Has to be set by subtype constructor
        protected readonly Mock<IResourcesRepository<TResource, TKey>> MockRepo;
        protected readonly Mock<ILogger<TController>> MockLogger;
        protected readonly Faker Faker;

        protected abstract TKey   GetKey(TResource resource);
        protected abstract TKey   MockKey();
        protected abstract TKey[] MockKeys(int count);

        protected virtual TResource MockResource(string recipeName, TKey key = default)
        {
            return new();
        }

        protected TResource MockResourceInRepository(string recipeName, TKey key = default)
        {
            if (key is null || key.Equals(default))
            {
                key = MockKey();
            }
            TResource mockedResource = MockResource(recipeName, key);

            MockRepo.Setup(repo => repo.GetAsync(recipeName, key))
                    .ReturnsAsync(mockedResource);
            MockRepo.Setup(repo => repo.ExistsAsync(recipeName, key))
                    .ReturnsAsync(true);
            MockRepo.Setup(repo => repo.CreateOrUpdateAsync(recipeName, mockedResource))
                    .ReturnsAsync(mockedResource);
            MockRepo.Setup(repo => repo.CreateOrUpdateAsync(recipeName, mockedResource))
                    .ReturnsAsync(mockedResource);

            return mockedResource;
        }

        protected IEnumerable<TResource> MockResourcesInRepository(string recipeName, int resourcesCount)
        {
            TKey[] keys = MockKeys(resourcesCount);
            return MockResourcesInRepository(recipeName, keys);
        }

        protected virtual IEnumerable<TResource> MockResourcesInRepository(string recipeName, params TKey[] ids)
        {
            // So that .ExistsAsync returns false to any resource not specified below.
            MockRepo.SetReturnsDefault(false);

            List<TResource> mockedResources = ids
                                              .Select(id => MockResourceInRepository(recipeName, id))
                                              .ToList();

            MockRepo.Setup(repo => repo.GetAllAsync(recipeName))
                    .ReturnsAsync(mockedResources);

            return mockedResources;
        }

        [Theory]
        [InlineData(1,  1)]
        [InlineData(1,  3)]
        [InlineData(1,  5)]
        [InlineData(1,  10)]
        [InlineData(3,  1)]
        [InlineData(3,  3)]
        [InlineData(3,  5)]
        [InlineData(3,  10)]
        [InlineData(10, 1)]
        [InlineData(10, 3)]
        [InlineData(10, 5)]
        [InlineData(10, 10)]
        protected async Task GetAll_ReturnsAllResourcesOnlyUnderRecipeName_WithMultipleRecipeResources(
            int recipeCount,
            int resourcesPerRecipe)
        {
            // Arrange
            var allRecipesResources = new Dictionary<string, IEnumerable<TResource>>();
            foreach (int _ in Enumerable.Range(0, recipeCount))
            {
                string                 recipeName = Faker.Lorem.Sentence();
                IEnumerable<TResource> resources  = MockResourcesInRepository(recipeName, resourcesPerRecipe);
                allRecipesResources.Add(recipeName, resources);
            }

            foreach (string recipeName in allRecipesResources.Keys)
            {
                IEnumerable<TResource> recipeResources = allRecipesResources[recipeName];

                // Act
                ActionResult<IEnumerable<TResource>> response = await Controller.GetAll(recipeName);

                // Assert
                response.Result.Should().BeAssignableTo<ObjectResult>();
                var objectResult = (ObjectResult) response.Result;

                objectResult.Value.Should().BeAssignableTo<IEnumerable<TResource>>();
                var resources = (IEnumerable<TResource>) objectResult.Value;

                resources.Should().BeSameAs(recipeResources);
            }
        }

        [Fact]
        protected async Task GetAll_ReturnsNoContent_WithNoResources()
        {
            // Arrange
            string recipeName = Faker.Lorem.Sentence();
            MockRepo.Setup(repository => repository.GetAllAsync(recipeName))
                    .ReturnsAsync(new List<TResource>());

            // Act
            ActionResult<IEnumerable<TResource>> response = await Controller.GetAll(recipeName);

            // Assert
            response.Result.Should().BeOfType<NoContentResult>();
        }

        [Fact]
        protected async Task Get_ReturnsResource_WithResources()
        {
            // Arrange
            string    recipeName = Faker.Lorem.Sentence();
            TResource resource   = MockResourceInRepository(recipeName);
            TKey      key        = GetKey(resource);

            // Act
            ActionResult<TResource> response = await Controller.Get(recipeName, key);

            // Assert
            response.Result.Should().BeAssignableTo<ObjectResult>();
            var objectResult = (ObjectResult) response.Result;

            objectResult.Value.Should().BeOfType<TResource>();
            var retrievedResource = (TResource) objectResult.Value;

            retrievedResource.Should().BeEquivalentTo(resource);
        }

        [Fact]
        protected async Task Get_ReturnsNotFound_WithNoResources()
        {
            // Arrange
            string    recipeName = Faker.Lorem.Sentence();
            TResource resource   = MockResource(recipeName);
            TKey      key        = GetKey(resource);

            // Act
            ActionResult<TResource> response = await Controller.Get(recipeName, key);

            // Assert
            response.Result.Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        protected async Task CreateOrUpdate_CreatesAtAction_WithRecipeWithNoResources()
        {
            // Arrange
            string recipeName = Faker.Lorem.Sentence();
            MockResourcesInRepository(recipeName, 0); // Creates empty resource list for recipeName.
            TResource newResource = MockResource(recipeName);

            MockRepo.Setup(repo => repo.CreateOrUpdateAsync(recipeName, newResource))
                    .ReturnsAsync(newResource);

            // Act
            ActionResult<TResource> response = await Controller.CreateOrUpdate(recipeName, newResource);

            // Assert
            response.Result.Should().BeAssignableTo<CreatedAtActionResult>();
            var objectResult = (CreatedAtActionResult) response.Result;

            // Needs to contain these keys so resource location is correct.
            objectResult.RouteValues.Keys.Should().Contain("id");
            objectResult.RouteValues.Keys.Should().Contain("recipeName");
            objectResult.ActionName.Should().Be(nameof(Controller.Get));
            
            objectResult.Value.Should().BeOfType<TResource>();
            var retrievedResource = (TResource) objectResult.Value;

            retrievedResource.Should().BeEquivalentTo(newResource);
        }

        [Fact]
        protected async Task CreateOrUpdate_UpdatesResource_WithExistingResource()
        {
            // Arrange
            string    recipeName       = Faker.Lorem.Sentence();
            TResource existingResource = MockResourceInRepository(recipeName);
            // Creates resource with new values but same key, just as if each property was updated.
            TResource updatedResource = MockResource(recipeName, GetKey(existingResource));

            MockRepo.Setup(repo => repo.CreateOrUpdateAsync(recipeName, updatedResource))
                    .ReturnsAsync(updatedResource);

            // Act
            ActionResult<TResource> response = await Controller.CreateOrUpdate(recipeName, updatedResource);

            // Assert
            response.Result.Should().BeAssignableTo<ObjectResult>();
            var objectResult = (ObjectResult) response.Result;

            objectResult.Value.Should().BeOfType<TResource>();
            var retrievedResource = (TResource) objectResult.Value;

            GetKey(retrievedResource).Should().BeEquivalentTo(GetKey(existingResource));
            retrievedResource.Should().NotBe(existingResource);
            retrievedResource.Should().BeEquivalentTo(updatedResource);
        }

        [Fact]
        protected async Task Delete_RemovesFromRepository_WhenResourceExists()
        {
            // Arrange
            string    recipeName = Faker.Lorem.Sentence();
            TResource resource   = MockResourceInRepository(recipeName);
            TKey      key        = GetKey(resource);

            // Act
            ActionResult<TResource> response = await Controller.Delete(recipeName, key);

            // Assert
            response.Result.Should().BeOfType<OkResult>();
        }

        [Fact]
        protected async Task Delete_ReturnsNotFound_WhenResourceDoesNotExist()
        {
            // Arrange
            string    recipeName = Faker.Lorem.Sentence();
            TResource resource   = MockResource(recipeName);
            TKey      key        = GetKey(resource);

            // Act
            ActionResult<TResource> response = await Controller.Delete(recipeName, key);

            // Assert
            response.Result.Should().BeOfType<NotFoundResult>();
        }
    }
}