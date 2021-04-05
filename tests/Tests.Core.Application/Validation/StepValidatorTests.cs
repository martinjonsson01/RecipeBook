using System.Collections.Generic;

using FluentAssertions;

using FluentValidation.Results;

using RecipeBook.Core.Application.Validation;
using RecipeBook.Core.Domain.Recipes;

using Xunit;

namespace Tests.Core.Application.Validation
{
    public class StepValidatorTests
    {
        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        [InlineData(-12)]
        [InlineData(-123)]
        private void Validate_Denies_NumbersLessThanOne(int number)
        {
            // Arrange
            Step        step  = new() { Number = number, Instruction = "test" };
            IList<Step> steps = new[] { step };

            // Act
            ValidationResult result = new StepsValidator().Validate(steps);

            // Assert
            result.Errors.Should().Contain(failure => failure.PropertyName.EndsWith(nameof(Step.Number)));
        }

        [Fact]
        private void Validate_Denies_EmptyInstruction()
        {
            // Arrange
            Step        step  = new() { Number = 1, Instruction = string.Empty };
            IList<Step> steps = new[] { step };

            // Act
            ValidationResult result = new StepsValidator().Validate(steps);

            // Assert
            result.Errors.Should().Contain(failure => failure.PropertyName.EndsWith(nameof(Step.Instruction)));
        }
    }
}