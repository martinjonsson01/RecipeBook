using FluentAssertions;

using RecipeBook.Core.Domain.Recipes;

using Tests.Shared;

using Xunit;

namespace Tests.Core.Application.Recipes
{
    public class RecipeTests
    {
        public static TheoryData<string, string> RecipeNames =>
            new()
            {
                { "test namn.!" , "test-namn.%21"},
                { "https://google.se", "https%3A%2F%2Fgoogle.se"},
                {"Grundrecept till färsbiffar/ köttbullar", "Grundrecept-till-f%C3%A4rsbiffar%2F%E2%80%89k%C3%B6ttbullar"}
            };
        
        [Theory]
        [MemberData(nameof(RecipeNames))]
        public void ToUrlSafeName_IsUrlSafe(string name, string safeName)
        {
            // Arrange
            var recipe = new Recipe
            {
                Name = name
            };

            // Act
            string actual     = recipe.ToUrlSafeName();

            // Assert
            actual.Should().BeEquivalentTo(safeName);
        }

        [Theory]
        [MemberData(nameof(RecipeNames))]
        public void FromUrlSafeNameToOrdinaryName_RecreatesOriginalName(string originalName, string safeName)
        {
            // Act
            string actual     = Recipe.FromUrlSafeNameToOrdinaryName(safeName);

            // Assert
            actual.Should().BeEquivalentTo(originalName);
        }

        [Fact]
        public void ShallowClone_ReturnsNewInstanceWithSameValues()
        {
            // Arrange
            Recipe recipe  = Fakers.Recipe.Generate();

            // Act
            Recipe shallowClone = recipe.ShallowClone();
            
            // Assert
            shallowClone.Should().NotBeSameAs(recipe);
            shallowClone.Should().BeEquivalentTo(recipe);
        }

        [Fact]
        public void Equals_ReturnsTrue_WhenRecipesAreEqual()
        {
            // Arrange
            Recipe first  = Fakers.Recipe.Generate();
            Recipe second = first.ShallowClone();

            // Act
            bool equal = first.Equals(second);

            // Assert
            equal.Should().BeTrue();
        }
        
        [Fact]
        public void Equals_ReturnsFalse_WhenRecipesAreDifferent()
        {
            // Arrange
            Recipe first = Fakers.Recipe.Generate();
            Recipe second = Fakers.Recipe.Generate();

            // Act
            bool equal = first.Equals(second);

            // Assert
            equal.Should().BeFalse();
        }
    }
}