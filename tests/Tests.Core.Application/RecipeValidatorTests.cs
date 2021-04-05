using System.Collections.Generic;
using System.Linq;

using FluentAssertions;

using FluentValidation.Results;

using RecipeBook.Core.Application;
using RecipeBook.Core.Domain.Recipes;

using Xunit;

namespace Tests.Core.Application
{
    public class RecipeValidatorTests
    {
        private readonly Recipe _invalidRecipe;

        public RecipeValidatorTests()
        {
            _invalidRecipe = new Recipe
            {
                Name = string.Empty,
                Rating = 11
            };
        }

        [Theory]
        [InlineData("  This is a valid name  ")]
        [InlineData("åöäöå äö åöä åöaåödf .-,-.,")]
        [InlineData("1")]
        private void Validate_Accepts_ValidName(string name)
        {
            // Arrange
            _invalidRecipe.Name = name;
            
            // Act
            ValidationResult result = new RecipeValidator().Validate(_invalidRecipe);

            // Assert
            result.Errors
                  .All(failure => failure.PropertyName.Equals(nameof(Recipe.Name)))
                  .Should().BeFalse();
        }

        [Theory]
        [InlineData("TTTTTTTTTTTTTTTTTTTTTTTTTTTTTTTTTTTTTTTTTTTTTTTTTTTTTTTTTTTTTTTTTTTTTTTTTTTTTTTTTTTTTTTTTTTTTTTTTTTTT")]
        [InlineData("")]
        private void Validate_Denies_InvalidName(string name)
        {
            // Arrange
            _invalidRecipe.Name = name;

            // Act
            ValidationResult result = new RecipeValidator().Validate(_invalidRecipe);

            // Assert
            result.Errors
                  .Any(failure => failure.PropertyName.Equals(nameof(Recipe.Name)))
                  .Should().BeTrue();
        }

        [Theory]
        [InlineData(1)]
        [InlineData(3)]
        [InlineData(6)]
        [InlineData(9)]
        [InlineData(10)]
        private void Validate_Accepts_ValidRating(int rating)
        {
            // Arrange
            _invalidRecipe.Rating = rating;

            // Act
            ValidationResult result = new RecipeValidator().Validate(_invalidRecipe);

            // Assert
            result.Errors
                  .All(failure => failure.PropertyName.Equals(nameof(Recipe.Rating)))
                  .Should().BeFalse();
        }

        [Theory]
        [InlineData(-12)]
        [InlineData(-1)]
        [InlineData(0)]
        [InlineData(11)]
        [InlineData(100)]
        private void Validate_Denies_InvalidRating(int? rating)
        {
            // Arrange
            _invalidRecipe.Rating = rating;

            // Act
            ValidationResult result = new RecipeValidator().Validate(_invalidRecipe);

            // Assert
            result.Errors
                  .Any(failure => failure.PropertyName.Equals(nameof(Recipe.Name)))
                  .Should().BeTrue();
        }
    }
}
