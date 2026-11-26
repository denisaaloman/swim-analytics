namespace SwimAnalytics.Domain.Enums;

public enum PacingProfile
{
    Even = 1,
    PositiveSplit = 2,   // a doua jumătate mai lentă (a plecat prea tare)
    NegativeSplit = 3    // a doua jumătate mai rapidă
}