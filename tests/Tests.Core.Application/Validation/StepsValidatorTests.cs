using System;
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

        public static TheoryData<TimeSpan> ValidStepDurations => new()
        {
            TimeSpan.FromMinutes(11),
            TimeSpan.FromHours(2),
            TimeSpan.FromDays(2)
        };

        [Theory]
        [MemberData(nameof(ValidStepDurations))]
        private void Validate_Accepts_ValidStepDurations(TimeSpan duration)
        {
            // Arrange
            TimeStep    step  = new() { Number = 1, Instruction = "instruction", Duration = duration };
            IList<Step> steps = new Step[] { step };

            // Act
            ValidationResult result = new StepsValidator().Validate(steps);

            // Assert
            result.Errors.Should().NotContain(failure => failure.PropertyName.EndsWith(nameof(TimeStep.Duration)));
        }

        public static TheoryData<TimeSpan> InvalidStepDurations => new()
        {
            TimeSpan.Zero,
            TimeSpan.FromDays(12),
            TimeSpan.MaxValue
        };

        [Theory]
        [MemberData(nameof(InvalidStepDurations))]
        private void Validate_Accepts_InvalidStepDurations(TimeSpan duration)
        {
            // Arrange
            TimeStep    step  = new() { Number = 1, Instruction = "instruction", Duration = duration };
            IList<Step> steps = new Step[] { step };

            // Act
            ValidationResult result = new StepsValidator().Validate(steps);

            // Assert
            result.Errors.Should().Contain(failure => failure.PropertyName.EndsWith(nameof(TimeStep.Duration)));
        }
    }
}