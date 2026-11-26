using SwimAnalytics.Application.Services;
using SwimAnalytics.Domain.Enums;
using Xunit;

namespace SwimAnalytics.Tests.Logic;

public class RelayOptimizerTests
{
    
    private static List<(int SwimmerId, Stroke Stroke, int TimeMs)> FromMatrix(
        params (int Swimmer, int Back, int Breast, int Fly, int Free)[] rows)
    {
        var list = new List<(int SwimmerId, Stroke Stroke, int TimeMs)>();
        foreach (var r in rows)
        {
            list.Add((r.Swimmer, Stroke.Backstroke, r.Back * 1000));
            list.Add((r.Swimmer, Stroke.Breaststroke, r.Breast * 1000));
            list.Add((r.Swimmer, Stroke.Butterfly, r.Fly * 1000));
            list.Add((r.Swimmer, Stroke.Freestyle, r.Free * 1000));
        }
        return list;
    }

    private static readonly (int, int, int, int, int)[] TrapMatrix =
    {
        (1, 50, 70, 70, 45),
        (2, 52, 65, 60, 46),
        (3, 60, 62, 66, 55),
        (4, 58, 68, 64, 52),
    };

    [Fact]
    public void Optimize_GreedyWouldReuseSwimmer_ReturnsOptimalDistinctAssignment()
    {
        var plan = RelayOptimizer.Optimize(FromMatrix(TrapMatrix));

        Assert.NotNull(plan);
        Assert.Equal(222_000, plan.TotalTimeMs);
        Assert.Equal(4, plan.Legs.Select(l => l.SwimmerId).Distinct().Count());

       
        Assert.Equal(new[] { 1, 3, 4, 2 }, plan.Legs.Select(l => l.SwimmerId));
    }

    [Fact]
    public void Optimize_LegsAreInMedleyOrder()
    {
        var plan = RelayOptimizer.Optimize(FromMatrix(TrapMatrix));

        Assert.NotNull(plan);
        Assert.Equal(
            new[] { Stroke.Backstroke, Stroke.Breaststroke, Stroke.Butterfly, Stroke.Freestyle },
            plan.Legs.Select(l => l.Stroke));
    }

    [Fact]
    public void Optimize_TotalEqualsSumOfLegTimes()
    {
        var plan = RelayOptimizer.Optimize(FromMatrix(TrapMatrix));

        Assert.NotNull(plan);
        Assert.Equal(plan.TotalTimeMs, plan.Legs.Sum(l => l.TimeMs));
    }

    [Fact]
    public void Optimize_MoreThanFourSwimmers_LeavesTheSlowOneOut()
    {
        var rows = TrapMatrix.Append((5, 70, 75, 72, 70)).ToArray();

        var plan = RelayOptimizer.Optimize(FromMatrix(rows));

        Assert.NotNull(plan);
        Assert.Equal(222_000, plan.TotalTimeMs);
        Assert.DoesNotContain(plan.Legs, l => l.SwimmerId == 5);
    }

    [Fact]
    public void Optimize_SwimmerWithSeveralResultsOnSameStroke_UsesTheBestOne()
    {
        var candidates = new List<(int SwimmerId, Stroke Stroke, int TimeMs)>
        {
            (1, Stroke.Backstroke, 60_000),
            (1, Stroke.Backstroke, 50_000), 
            (2, Stroke.Breaststroke, 60_000),
            (3, Stroke.Butterfly, 60_000),
            (4, Stroke.Freestyle, 60_000),
        };

        var plan = RelayOptimizer.Optimize(candidates);

        Assert.NotNull(plan);
        Assert.Equal(50_000, plan.Legs[0].TimeMs);
        Assert.Equal(230_000, plan.TotalTimeMs);
    }

    [Fact]
    public void Optimize_FewerThanFourSwimmers_ReturnsNull()
    {
       
        var plan = RelayOptimizer.Optimize(FromMatrix(
            (1, 50, 70, 70, 45),
            (2, 52, 65, 60, 46),
            (3, 60, 62, 66, 55)));

        Assert.Null(plan);
    }

    [Fact]
    public void Optimize_NobodySwimsOneStroke_ReturnsNull()
    {
        var candidates = new List<(int SwimmerId, Stroke Stroke, int TimeMs)>
        {
            (1, Stroke.Backstroke, 50_000),
            (2, Stroke.Breaststroke, 60_000),
            (3, Stroke.Freestyle, 55_000),
            (4, Stroke.Freestyle, 56_000), 
        };

        Assert.Null(RelayOptimizer.Optimize(candidates));
    }

    [Fact]
    public void Optimize_NoCandidates_ReturnsNull()
    {
        Assert.Null(RelayOptimizer.Optimize(Array.Empty<(int, Stroke, int)>()));
    }
}
