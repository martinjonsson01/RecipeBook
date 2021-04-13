using System;

using Bogus;

using FluentAssertions;

using RecipeBook.Core.Domain.Recipes;

using Tests.Shared;

using Xunit;

namespace Tests.Core.Application.Recipes
{
    public class RecipeTests
    {
        [Fact]
        public void ToUrlSafeName_IsUrlSafe()
        {
            // Arrange
            var recipe = new Recipe
            {
                Name = new Faker("sv").Lorem.Sentence()
            };

            // Act
            string actual     = recipe.ToUrlSafeName();
            string actualSafe = Uri.EscapeUriString(actual);

            // Assert
            actual.Should().BeEquivalentTo(actualSafe);
        }

        [Fact]
        public void FromUrlSafeNameToOrdinaryName_RecreatesOriginalName()
        {
            // Arrange
            string name    = new Faker("sv").Lorem.Sentence();
            string urlSafe = Uri.EscapeUriString(name);

            // Act
            string actual     = Recipe.FromUrlSafeNameToOrdinaryName(urlSafe);

            // Assert
            actual.Should().BeEquivalentTo(name);
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