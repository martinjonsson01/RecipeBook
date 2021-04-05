﻿using System;
using System.Collections.Generic;
using System.Linq;

using FluentAssertions;

using FluentValidation.Results;

using RecipeBook.Core.Application;
using RecipeBook.Core.Domain.Recipes;

using Xunit;

namespace Tests.Core.Application
{
    public class UsedOccasionValidatorTests
    {
        private readonly UsedOccasion _invalidUsedOccasion;

        public UsedOccasionValidatorTests()
        {
            _invalidUsedOccasion = new UsedOccasion
            {
                When = DateTime.MinValue,
                Duration = TimeSpan.FromDays(2),
                Comment = new string('T', 501)
            };
        }

        public static TheoryData<TimeSpan> ValidDurations => new()
        {
            TimeSpan.FromHours(1)
        };

        [Theory]
        [MemberData(nameof(ValidDurations))]
        private void Validate_Accepts_ValidDurations(TimeSpan duration)
        {
            // Arrange
            _invalidUsedOccasion.Duration = duration;

            // Act
            ValidationResult result = new UsedOccasionValidator().Validate(_invalidUsedOccasion);

            // Assert
            result.Errors
                  .Should()
                  .NotContain(failure => failure.PropertyName.Equals(nameof(UsedOccasion.Duration)));
        }

        public static TheoryData<TimeSpan> InvalidDurations => new()
        {
            TimeSpan.Zero,
            TimeSpan.FromDays(1)
        };

        [Theory]
        [MemberData(nameof(InvalidDurations))]
        private void Validate_Denies_InvalidDurations(TimeSpan duration)
        {
            // Arrange
            _invalidUsedOccasion.Duration = duration;

            // Act
            ValidationResult result = new UsedOccasionValidator().Validate(_invalidUsedOccasion);

            // Assert
            result.Errors
                  .Should()
                  .Contain(failure => failure.PropertyName.Equals(nameof(UsedOccasion.Duration)));
        }

        [Theory]
        [InlineData("  This is a valid comment  ")]
        [InlineData("åöäöå äö åöä åöaåödf .-,-.,")]
        [InlineData("1")]
        private void Validate_Accepts_ValidComments(string comment)
        {
            // Arrange
            _invalidUsedOccasion.Comment = comment;

            // Act
            ValidationResult result = new UsedOccasionValidator().Validate(_invalidUsedOccasion);

            // Assert
            result.Errors
                  .Should()
                  .NotContain(failure => failure.PropertyName.Equals(nameof(UsedOccasion.Comment)));
        }

        [Fact]
        private void Validate_Denies_TooLongComment()
        {
            // Arrange
            _invalidUsedOccasion.Comment = new string('T', 501);

            // Act
            ValidationResult result = new UsedOccasionValidator().Validate(_invalidUsedOccasion);

            // Assert
            result.Errors
                  .Should()
                  .Contain(failure => failure.PropertyName.Equals(nameof(UsedOccasion.Comment)));
        }
    }
}