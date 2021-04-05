using System;
using System.Collections.Generic;

using FluentAssertions;

using FluentValidation.Results;

using RecipeBook.Core.Application.Validation;
using RecipeBook.Core.Domain.Recipes;

using Xunit;

namespace Tests.Core.Application.Validation
{
    public class TimeStepValidatorTests
    {
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