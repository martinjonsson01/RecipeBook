using System.Collections.Generic;
using System.Linq;

using FluentAssertions;

using FluentValidation.Results;

using RecipeBook.Core.Application.Validation;
using RecipeBook.Core.Domain.Recipes;

using Xunit;

namespace Tests.Core.Application.Validation
{
    public class StepsValidatorTests
    {
        [Fact]
        private void Validate_Denies_EmptySteps()
        {
            // Arrange
            var steps = new List<Step>();

            // Act
            ValidationResult result = new StepsValidator().Validate(steps);

            // Assert
            result.Errors.Should().Contain(failure => failure.PropertyName.Equals(string.Empty));
        }

        public static TheoryData<int[]> ValidNumbers => new()
        {
            new[] { 1, 2, 3, 4 },
            new[] { 1, 2, 3, 4, 5, 6, 7 },
            new[] { 1, 2 },
            new[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13 }
        };

        [Theory]
        [MemberData(nameof(ValidNumbers))]
        private void Validate_Accepts_ValidNumbers(int[] numbers)
        {
            // Arrange
            IList<Step> steps = numbers
                                .Select(number => new Step { Number = number })
                                .ToList();

            // Act
            ValidationResult result = new StepsValidator().Validate(steps);

            // Assert
            result.Errors.Should().NotContain(failure => failure.PropertyName.Equals(string.Empty));
        }

        public static TheoryData<int[]> InvalidNumbers => new()
        {
            new[] { 0, 1, 2, 3, 4 },
            new[] { 1, 3, 4, 5, 7 },
            new[] { 0 },
            new[] { 4, 5, 6, 7, 8, 9, 10, 11, 12, 13 }
        };

        [Theory]
        [MemberData(nameof(InvalidNumbers))]
        private void Validate_Denies_InvalidNumbers(int[] numbers)
        {
            // Arrange
            IList<Step> steps = numbers
                                .Select(number => new Step { Number = number })
                                .ToList();

            // Act
            ValidationResult result = new StepsValidator().Validate(steps);

            // Assert
            result.Errors.Should().Contain(failure => failure.PropertyName.Equals(string.Empty));
        }
    }
}