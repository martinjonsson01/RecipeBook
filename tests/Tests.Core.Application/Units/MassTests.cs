using System.Globalization;

using FluentAssertions;

using RecipeBook.Core.Domain.Units;

using Xunit;

namespace Tests.Core.Application.Units
{
    public class MassTests
    {
        public MassTests()
        {
            CultureInfo.CurrentCulture = new CultureInfo("sv-SE");
        }
        
        [Theory]
        [InlineData(0,    0d)]
        [InlineData(1,    0.001d)]
        [InlineData(100,  0.1d)]
        [InlineData(1000, 1.0d)]
        [InlineData(2345, 2.345d)]
        public void FromGrams_ConvertsToKilograms(int grams, double expectedKilograms)
        {
            // Act
            Mass   mass            = Mass.FromGrams(grams);
            double actualKilograms = mass.Value;

            // Assert
            actualKilograms.Should().Be(expectedKilograms);
        }

        [Theory]
        [InlineData(0,   "0 g")]
        [InlineData(10,  "10 g")]
        [InlineData(234, "234 g")]
        [InlineData(999, "999 g")]
        public void ToString_DisplaysInGrams_ValuesUnder1Kg(int grams, string expected)
        {
            // Act
            Mass mass   = Mass.FromGrams(grams);
            var  actual = mass.ToString();

            // Assert
            actual.Should().Be(expected);
        }

        [Theory]
        [InlineData(1000,  "1 kg")]
        [InlineData(1234,  "1,234 kg")]
        [InlineData(2500,  "2,5 kg")]
        [InlineData(10001, "10,001 kg")]
        public void ToString_DisplaysInKilograms_ValuesAbove1Kg(int grams, string expected)
        {
            // Act
            Mass mass   = Mass.FromGrams(grams);
            var  actual = mass.ToString();

            // Assert
            actual.Should().Be(expected);
        }
    }
}