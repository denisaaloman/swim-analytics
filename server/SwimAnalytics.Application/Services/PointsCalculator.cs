namespace SwimAnalytics.Application.Services;

public static class PointsCalculator
{
    // formula World Aquatics: 1000 * (timp_de_baza / timp)^3
    public static int Calculate(int baseTimeMs, int timeMs)
    {
        if (baseTimeMs <= 0) throw new ArgumentOutOfRangeException(nameof(baseTimeMs));
        if (timeMs <= 0) throw new ArgumentOutOfRangeException(nameof(timeMs));

        var ratio = (double)baseTimeMs / timeMs;
        return (int)Math.Floor(1000 * Math.Pow(ratio, 3));
    }
}