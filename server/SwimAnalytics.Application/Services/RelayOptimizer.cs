using SwimAnalytics.Domain.Enums;

namespace SwimAnalytics.Application.Services;

public record RelayAssignment(Stroke Stroke, int SwimmerId, int TimeMs);

public record RelayPlan(int TotalTimeMs, IReadOnlyList<RelayAssignment> Legs);

public static class RelayOptimizer
{
    
    public static readonly Stroke[] LegOrder =
        { Stroke.Backstroke, Stroke.Breaststroke, Stroke.Butterfly, Stroke.Freestyle };

    private const int FullMask = (1 << 4) - 1;
    private const long Inf = long.MaxValue / 4;

    
    public static RelayPlan? Optimize(IEnumerable<(int SwimmerId, Stroke Stroke, int TimeMs)> candidates)
    {
        var swimmers = candidates
            .GroupBy(c => c.SwimmerId)
            .Select(g => (Id: g.Key, Times: BuildTimes(g.Select(x => (x.Stroke, x.TimeMs)))))
            .ToList();

       
        var dp = new long[FullMask + 1];
        Array.Fill(dp, Inf);
        dp[0] = 0;

        
        var choice = new int[swimmers.Count][];

        for (var i = 0; i < swimmers.Count; i++)
        {
            choice[i] = new int[FullMask + 1];
            Array.Fill(choice[i], -1);

            var next = (long[])dp.Clone();

            for (var mask = 0; mask <= FullMask; mask++)
            {
                if (dp[mask] >= Inf) continue;

                for (var leg = 0; leg < LegOrder.Length; leg++)
                {
                    if ((mask & (1 << leg)) != 0) continue;

                    var time = swimmers[i].Times[leg];
                    if (time == int.MaxValue) continue;

                    var newMask = mask | (1 << leg);
                    var total = dp[mask] + time;

                    if (total < next[newMask])
                    {
                        next[newMask] = total;
                        choice[i][newMask] = leg;
                    }
                }
            }

            dp = next;
        }

        if (dp[FullMask] >= Inf) return null;

        var legs = new List<(int LegIndex, RelayAssignment Assignment)>();
        var current = FullMask;

        for (var i = swimmers.Count - 1; i >= 0; i--)
        {
            var leg = choice[i][current];
            if (leg < 0) continue;

            legs.Add((leg, new RelayAssignment(LegOrder[leg], swimmers[i].Id, swimmers[i].Times[leg])));
            current ^= 1 << leg;
        }

        var ordered = legs.OrderBy(l => l.LegIndex).Select(l => l.Assignment).ToList();
        return new RelayPlan((int)dp[FullMask], ordered);
    }

    private static int[] BuildTimes(IEnumerable<(Stroke Stroke, int TimeMs)> results)
    {
        var times = Enumerable.Repeat(int.MaxValue, LegOrder.Length).ToArray();

        foreach (var (stroke, timeMs) in results)
        {
            var leg = Array.IndexOf(LegOrder, stroke);
            if (leg >= 0 && timeMs < times[leg]) times[leg] = timeMs;
        }

        return times;
    }
}