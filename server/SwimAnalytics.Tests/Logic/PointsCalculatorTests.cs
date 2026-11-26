using SwimAnalytics.Application.Services;
using Xunit;

namespace SwimAnalytics.Tests.Logic;

public class PointsCalculatorTests
{
    [Fact]
    public void Calculate_TimeEqualsBaseTime_Returns1000()
    {
        Assert.Equal(1000, PointsCalculator.Calculate(50_000, 50_000));
    }

    [Fact]
    public void Calculate_FasterThanBaseTime_ReturnsMoreThan1000()
    {
       
        Assert.Equal(1371, PointsCalculator.Calculate(50_000, 45_000));
    }

    [Fact]
    public void Calculate_SlowerThanBaseTime_ReturnsLessThan1000()
    {
        
        Assert.Equal(751, PointsCalculator.Calculate(50_000, 55_000));
    }

    [Fact]
    public void Calculate_FasterTimeAlwaysGetsMorePoints()
    {
        var slower = PointsCalculator.Calculate(50_000, 49_500);
        var faster = PointsCalculator.Calculate(50_000, 49_000);

        Assert.True(faster > slower);
    }

    [Theory]
    [InlineData(0, 50_000)]
    [InlineData(-1, 50_000)]
    [InlineData(50_000, 0)]
    [InlineData(50_000, -5)]
    public void Calculate_NonPositiveInput_Throws(int baseTimeMs, int timeMs)
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => PointsCalculator.Calculate(baseTimeMs, timeMs));
    }
}
