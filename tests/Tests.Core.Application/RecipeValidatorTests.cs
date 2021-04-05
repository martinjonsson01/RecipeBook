using System.Collections.Generic;

using FluentAssertions;

using FluentValidation.Results;

using RecipeBook.Core.Application;
using RecipeBook.Core.Domain.Recipes;

using Xunit;

namespace Tests.Core.Application
{
    public class RecipeValidatorTests
    {
        [Fact]
        private void ValidateRecipe_Accepts_ValidRecipe()
        {
            // Arrange
            var validRecipe = new Recipe
            {
                Name = "This is a valid name",
                Rating = 6,
                UsedOccasions = new List<UsedOccasion>(),
                Steps = new List<Step>(),
                Ingredients = new List<Ingredient>()
            };   
            
            // Act
            ValidationResult result = new RecipeValidator().Validate(validRecipe);

            // Assert
            result.IsValid.Should().BeTrue();
        }

        [Fact]
        private void ValidateRecipe_Denies_InvalidRecipe()
        {
            // Arrange
            var invalidRecipe = new Recipe
            {
                Name = string.Empty,
                Rating = 11
            };

            // Act
            ValidationResult result = new RecipeValidator().Validate(invalidRecipe);

            // Assert
            result.IsValid.Should().BeFalse();
        }
    }
}
