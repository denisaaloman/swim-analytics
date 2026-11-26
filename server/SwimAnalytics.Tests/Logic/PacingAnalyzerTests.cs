using SwimAnalytics.Application.Services;
using SwimAnalytics.Domain.Enums;
using Xunit;

namespace SwimAnalytics.Tests.Logic;

public class PacingAnalyzerTests
{
    [Fact]
    public void Analyze_NoSplits_ReturnsNull()
    {
        var result = PacingAnalyzer.Analyze(100, 54_000, Array.Empty<(int, int)>());

        Assert.Null(result);
    }

    [Fact]
    public void Analyze_OnlySplitAtEventDistance_IsIgnoredAndReturnsNull()
    {
        
        var result = PacingAnalyzer.Analyze(100, 54_000, new[] { (100, 54_000) });

        Assert.Null(result);
    }

    [Fact]
    public void Analyze_EqualHalves_IsEven()
    {
        var result = PacingAnalyzer.Analyze(100, 54_000, new[] { (50, 27_000) });

        Assert.NotNull(result);
        Assert.Equal(27_000, result.FirstHalfMs);
        Assert.Equal(27_000, result.SecondHalfMs);
        Assert.Equal(0, result.DifferencePercent, 2);
        Assert.Equal(PacingProfile.Even, result.Profile);
    }

    [Fact]
    public void Analyze_SlowerSecondHalf_IsPositiveSplit()
    {
        var result = PacingAnalyzer.Analyze(100, 56_000, new[] { (50, 26_000) });

        Assert.NotNull(result);
        Assert.Equal(26_000, result.FirstHalfMs);
        Assert.Equal(30_000, result.SecondHalfMs);
        Assert.Equal(15.38, result.DifferencePercent, 2);
        Assert.Equal(PacingProfile.PositiveSplit, result.Profile);
    }

    [Fact]
    public void Analyze_FasterSecondHalf_IsNegativeSplit()
    {
        var result = PacingAnalyzer.Analyze(100, 54_000, new[] { (50, 28_000) });

        Assert.NotNull(result);
        Assert.Equal(-7.14, result.DifferencePercent, 2);
        Assert.Equal(PacingProfile.NegativeSplit, result.Profile);
    }

    [Fact]
    public void Analyze_SmallDifferenceUnderOnePercent_IsStillEven()
    {
     
        var result = PacingAnalyzer.Analyze(100, 54_200, new[] { (50, 27_000) });

        Assert.NotNull(result);
        Assert.Equal(0.74, result.DifferencePercent, 2);
        Assert.Equal(PacingProfile.Even, result.Profile);
    }

    [Fact]
    public void Analyze_200mWithAllSplits_BuildsFourSegments()
    {
        var splits = new[] { (50, 27_000), (100, 56_000), (150, 85_000) };

        var result = PacingAnalyzer.Analyze(200, 114_000, splits);

        Assert.NotNull(result);
        Assert.Equal(4, result.Segments.Count);
        Assert.Equal(new[] { 27_000, 29_000, 29_000, 29_000 }, result.Segments.Select(s => s.TimeMs));
        Assert.Equal(56_000, result.FirstHalfMs);
        Assert.Equal(58_000, result.SecondHalfMs);
        Assert.Equal(PacingProfile.PositiveSplit, result.Profile);
    }

    [Fact]
    public void Analyze_UnorderedSplits_AreSortedByDistance()
    {
        var ordered = PacingAnalyzer.Analyze(200, 114_000, new[] { (50, 27_000), (100, 56_000), (150, 85_000) });
        var shuffled = PacingAnalyzer.Analyze(200, 114_000, new[] { (150, 85_000), (50, 27_000), (100, 56_000) });

        Assert.NotNull(ordered);
        Assert.NotNull(shuffled);
        Assert.Equal(ordered.Segments.Select(s => s.TimeMs), shuffled.Segments.Select(s => s.TimeMs));
    }

    [Fact]
    public void Analyze_MissingSplitAtHalf_InterpolatesLinearly()
    {
       
        var result = PacingAnalyzer.Analyze(200, 120_000, new[] { (50, 28_000), (150, 88_000) });

        Assert.NotNull(result);
        Assert.Equal(58_000, result.FirstHalfMs);
        Assert.Equal(62_000, result.SecondHalfMs);
        Assert.Equal(6.9, result.DifferencePercent, 2);
    }

    [Fact]
    public void Analyze_SegmentLongerThan50m_IsNormalizedPer50m()
    {
        var result = PacingAnalyzer.Analyze(200, 120_000, new[] { (50, 28_000), (150, 88_000) });

        Assert.NotNull(result);
        var middle = result.Segments[1]; // 50m -> 150m, 60000 ms pe 100m
        Assert.Equal(50, middle.FromMeters);
        Assert.Equal(150, middle.ToMeters);
        Assert.Equal(60_000, middle.TimeMs);
        Assert.Equal(30_000, middle.Per50Ms, 1);
    }
}
