using System.Globalization;

using FluentAssertions;

using RecipeBook.Core.Domain.Units;

using Xunit;

namespace Tests.Core.Application.Units
{
    public class UnitTests
    {
        private const double Precision = 0.0001d;

        public UnitTests()
        {
            CultureInfo.CurrentCulture = new CultureInfo("sv-SE");
        }
        
        [Theory]
        [InlineData("asdasdg")]
        [InlineData("34234 asdfasd")]
        [InlineData("334.,3.1 kg")]
        [InlineData("21,345 tg")]
        [InlineData("21,345 jl")]
        [InlineData("21,345 gh")]
        [InlineData("21,345 ly")]
        [InlineData("01l")]
        [InlineData("")]
        public void TryParseString_ShouldReturnFalseAndErrorMessage_WithIncorrectStrings(string s)
        {
            // Act
            bool result = Unit.TryParseString(s, out Unit _, out string? errorMessage);
            
            // Assert
            result.Should().BeFalse();
            errorMessage.Should().NotBeEmpty();
        }

        [Theory]
        [InlineData("21,345 kg")]
        [InlineData("21,345 g")]
        [InlineData("  21,345 g    ")]
        public void TryParseString_ShouldReturnTrueAndMass_WithCorrectMassStrings(string s)
        {
            // Act
            bool result = Unit.TryParseString(s, out Unit unit, out string? errorMessage);

            // Assert
            result.Should().BeTrue();
            unit.Should().BeAssignableTo<Mass>();
            errorMessage.Should().BeNull();
        }

        [Theory]
        [InlineData("   21,345 l    ")]
        [InlineData("21,345 l")]
        [InlineData("21,345 dl")]
        [InlineData("21,345 cl")]
        [InlineData("21,345 ml")]
        [InlineData("21,345 krm")]
        [InlineData("21,345 tsk")]
        [InlineData("21,345 msk")]
        public void TryParseString_ShouldReturnTrueAndVolume_WithCorrectVolumeStrings(string s)
        {
            // Act
            bool result = Unit.TryParseString(s, out Unit unit, out string? errorMessage);

            // Assert
            result.Should().BeTrue();
            unit.Should().BeAssignableTo<Volume>();
            errorMessage.Should().BeNull();
        }

        [Theory]
        [InlineData("21,345 kg", 21.345d)]
        [InlineData("21,345 g", 0.021345d)]
        public void TryParseString_ShouldReturnValueInKilograms_WithCorrectMassStrings(string s, double expectedKilograms)
        {
            // Act
            Unit.TryParseString(s, out Unit unit, out string? _);

            // Assert
            unit.Value.Should().BeApproximately(expectedKilograms, Precision);
        }

        [Theory]
        [InlineData("21,345 l",   21.345d)]
        [InlineData("21,345 dl",  2.1345d)]
        [InlineData("21,345 cl",  0.21345d)]
        [InlineData("21,345 ml",  0.021345d)]
        [InlineData("21,345 krm", 0.021345d)]
        [InlineData("21,345 tsk", 0.1067250d)]
        [InlineData("21,345 msk", 0.3201750d)]
        public void TryParseString_ShouldReturnValueInLiters_WithCorrectVolumeStrings(string s, double expectedLiters)
        {
            // Act
            Unit.TryParseString(s, out Unit unit, out string? _);

            // Assert
            unit.Value.Should().BeApproximately(expectedLiters, Precision);
        }
    }
}