using FluentAssertions;

using FluentValidation.Results;

using RecipeBook.Core.Application.Validators;
using RecipeBook.Core.Domain.Recipes;

using Xunit;

namespace Tests.Core.Application.Validation
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
            result.Errors.Should().NotContain(nameof(Recipe.Name));
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
                  .Should()
                  .ContainSingle(failure => failure.PropertyName.Equals(nameof(Recipe.Name)));
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
                  .Should()
                  .NotContain(failure => failure.PropertyName.Equals(nameof(Recipe.Rating)));
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
                  .Should()
                  .ContainSingle(failure => failure.PropertyName.Equals(nameof(Recipe.Rating)));
        }
    }
}
