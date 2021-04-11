using System;

using Bogus;

using FluentAssertions;

using RecipeBook.Core.Domain.Recipes;

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
    }
}