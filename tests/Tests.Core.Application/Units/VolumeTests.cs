using FluentAssertions;

using RecipeBook.Core.Domain.Units;

using Xunit;

namespace Tests.Core.Application.Units
{
    public class VolumeTests
    {
        [Theory]
        [InlineData(1,  0.015d)]
        [InlineData(3,  0.045d)]
        [InlineData(10, 0.15d)]
        public void FromTableSpoons_ConvertsToLitersCorrectly(int tableSpoons, double expectedLiters)
        {
            // Act
            Volume volume       = Volume.FromTableSpoons(tableSpoons);
            double actualLiters = volume.Value;

            // Assert
            actualLiters.Should().Be(expectedLiters);
        }

        [Theory]
        [InlineData(1,  0.005d)]
        [InlineData(3,  0.015d)]
        [InlineData(10, 0.05d)]
        public void FromTeaSpoons_ConvertsToLitersCorrectly(int teaSpoons, double expectedLiters)
        {
            // Act
            Volume volume       = Volume.FromTeaSpoons(teaSpoons);
            double actualLiters = volume.Value;

            // Assert
            actualLiters.Should().Be(expectedLiters);
        }

        [Theory]
        [InlineData(1,  0.001d)]
        [InlineData(3,  0.003d)]
        [InlineData(10, 0.01d)]
        public void FromMilliliters_ConvertsToLitersCorrectly(int milliliters, double expectedLiters)
        {
            // Act
            Volume volume       = Volume.FromMilliliters(milliliters);
            double actualLiters = volume.Value;

            // Assert
            actualLiters.Should().Be(expectedLiters);
        }

        [Theory]
        [InlineData(1,  0.001d)]
        [InlineData(3,  0.003d)]
        [InlineData(10, 0.01d)]
        public void FromKrm_ConvertsToLitersCorrectly(int krm, double expectedLiters)
        {
            // Act
            Volume volume       = Volume.FromKrm(krm);
            double actualLiters = volume.Value;

            // Assert
            actualLiters.Should().Be(expectedLiters);
        }

        [Theory]
        [InlineData(3,  "3 ml")]
        [InlineData(12, "12 ml")]
        [InlineData(56, "56 ml")]
        [InlineData(99, "99 ml")]
        public void ToString_DisplaysInMilliliters(int milliliters, string expected)
        {
            // Act
            Volume volume = Volume.FromMilliliters(milliliters);
            var    actual = volume.ToString();

            // Assert
            actual.Should().Be(expected);
        }

        [Theory]
        [InlineData(3,  "3 cl")]
        [InlineData(12, "12 cl")]
        [InlineData(56, "56 cl")]
        [InlineData(99, "99 cl")]
        public void ToString_DisplaysInCentiliters(int centiliters, string expected)
        {
            // Act
            Volume volume = Volume.FromMilliliters(centiliters * 10);
            var    actual = volume.ToString();

            // Assert
            actual.Should().Be(expected);
        }

        [Theory]
        [InlineData(3,  "3 dl")]
        [InlineData(12, "12 dl")]
        [InlineData(56, "56 dl")]
        [InlineData(99, "99 dl")]
        public void ToString_DisplaysInDeciliters(int deciliters, string expected)
        {
            // Act
            Volume volume = Volume.FromMilliliters(deciliters * 100);
            var    actual = volume.ToString();

            // Assert
            actual.Should().Be(expected);
        }

        [Theory]
        [InlineData(3,  "3 l")]
        [InlineData(12, "12 l")]
        [InlineData(56, "56 l")]
        [InlineData(99, "99 l")]
        public void ToString_DisplaysInLiters(int liters, string expected)
        {
            // Act
            var volume = new Volume(liters);
            var actual = volume.ToString();

            // Assert
            actual.Should().Be(expected);
        }
    }
}