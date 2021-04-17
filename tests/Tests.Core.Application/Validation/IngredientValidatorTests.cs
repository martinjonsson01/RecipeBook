using FluentAssertions;

using FluentValidation.Results;

using RecipeBook.Core.Application.Validators;
using RecipeBook.Core.Domain.Recipes;
using RecipeBook.Core.Domain.Units;

using Xunit;

namespace Tests.Core.Application.Validation
{
    public class IngredientValidatorTests
    {
        public static TheoryData<string> ValidNames => new()
        {
            "Name",
            "n",
            "1",
            "aödjföladfölkj 'ä¨äää'1 2133 lksdjfölkj ",
            "        öljlk          lökj   lökj     "
        };

        [Theory]
        [MemberData(nameof(ValidNames))]
        private void Validate_Accepts_ValidNames(string name)
        {
            // Arrange
            var ingredient = new Ingredient { Name = name, Amount = new Mass(0) };

            // Act
            ValidationResult result = new IngredientValidator().Validate(ingredient);

            // Assert
            result.Errors
                  .Should()
                  .NotContain(failure => failure.PropertyName.Equals(nameof(Ingredient.Name)));
        }

        public static TheoryData<string> InvalidNames => new()
        {
            "",
            new string('T', 101)
        };

        [Theory]
        [MemberData(nameof(InvalidNames))]
        private void Validate_Denies_InvalidNames(string name)
        {
            // Arrange
            var ingredient = new Ingredient { Name = name, Amount = new Mass(0) };

            // Act
            ValidationResult result = new IngredientValidator().Validate(ingredient);

            // Assert
            result.Errors
                  .Should()
                  .Contain(failure => failure.PropertyName.Equals(nameof(Ingredient.Name)));
        }

        [Theory]
        [InlineData(0.1d)]
        [InlineData(1d)]
        [InlineData(10.1d)]
        [InlineData(1202.3d)]
        private void Validate_Accepts_ValidUnitAmounts(double amount)
        {
            // Arrange
            var ingredient = new Ingredient { Name = "name", Amount = new Mass(amount) };

            // Act
            ValidationResult result = new IngredientValidator().Validate(ingredient);

            // Assert
            result.Errors
                  .Should()
                  .NotContain(failure => failure.PropertyName.Equals(nameof(Unit.Value)));
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1d)]
        [InlineData(-10.1d)]
        [InlineData(-1202.3d)]
        private void Validate_Denies_InvalidUnitAmount(double amount)
        {
            // Arrange
            var ingredient = new Ingredient { Name = "name", Amount = new Mass(amount) };

            // Act
            ValidationResult result = new IngredientValidator().Validate(ingredient);

            // Assert
            result.Errors
                  .Should()
                  .Contain(failure => failure.PropertyName.Equals($"{nameof(Ingredient.Amount)}.{nameof(Unit.Value)}"));
        }
    }
}