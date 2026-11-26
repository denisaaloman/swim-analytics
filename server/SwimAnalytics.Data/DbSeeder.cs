using Bogus;
using Microsoft.EntityFrameworkCore;
using SwimAnalytics.Domain.Entities;
using SwimAnalytics.Domain.Enums;

namespace SwimAnalytics.Data.Seeding;

public static class DbSeeder
{
    private static readonly (Stroke Stroke, int Distance, double Men, double Women)[] Catalog =
    {
        (Stroke.Freestyle, 50, 20.9, 23.7),
        (Stroke.Freestyle, 100, 46.4, 51.7),
        (Stroke.Freestyle, 200, 102.0, 112.0),
        (Stroke.Freestyle, 400, 219.0, 235.0),
        (Stroke.Backstroke, 50, 23.5, 26.3),
        (Stroke.Backstroke, 100, 51.0, 57.1),
        (Stroke.Backstroke, 200, 111.0, 123.0),
        (Stroke.Breaststroke, 50, 25.9, 29.2),
        (Stroke.Breaststroke, 100, 56.9, 64.1),
        (Stroke.Breaststroke, 200, 125.0, 138.0),
        (Stroke.Butterfly, 50, 22.3, 24.4),
        (Stroke.Butterfly, 100, 49.5, 54.6),
        (Stroke.Butterfly, 200, 110.0, 120.0),
        (Stroke.Medley, 200, 112.0, 125.0),
        (Stroke.Medley, 400, 240.0, 262.0),
    };

    private const double ShortCourseFactor = 0.97;

    public static async Task SeedAsync(AppDbContext db, int swimmerCount = 200)
    {
        if (await db.Events.AnyAsync()) return; // deja populat

        Randomizer.Seed = new Random(1234); // aceleași date la fiecare rulare
        var faker = new Faker("ro");
        var rnd = faker.Random;

        // Events + BaseTimes
        var events = new List<(Event Event, double BaseSec)>();
        foreach (var c in Catalog)
        {
            foreach (var gender in new[] { Gender.Male, Gender.Female })
            {
                var baseSec = gender == Gender.Male ? c.Men : c.Women;
                var ev = new Event { Stroke = c.Stroke, DistanceMeters = c.Distance, Gender = gender };

                ev.BaseTimes.Add(new BaseTime { Event = ev, PoolLength = PoolLength.Long, BaseTimeMs = (int)(baseSec * 1000) });
                ev.BaseTimes.Add(new BaseTime { Event = ev, PoolLength = PoolLength.Short, BaseTimeMs = (int)(baseSec * 1000 * ShortCourseFactor) });

                events.Add((ev, baseSec));
            }
        }

        // Clubs
        var clubs = Enumerable.Range(0, 10)
            .Select(_ =>
            {
                var city = faker.Address.City();
                return new Club { Name = $"CS {city}", City = city };
            })
            .ToList();

        // Meets
        var meets = Enumerable.Range(0, 15)
            .Select(i => new Meet
            {
                Name = $"Cupa {faker.Address.City()} {i + 1}",
                City = faker.Address.City(),
                Date = DateOnly.FromDateTime(faker.Date.Past(2)),
                PoolLength = i % 2 == 0 ? PoolLength.Long : PoolLength.Short
            })
            .ToList();

        // Swimmers + Results + Splits
        var swimmers = new List<Swimmer>();
        var results = new List<Result>();

        for (var i = 0; i < swimmerCount; i++)
        {
            var gender = rnd.Bool() ? Gender.Male : Gender.Female;
            var swimmer = new Swimmer
            {
                FirstName = faker.Name.FirstName(gender == Gender.Male ? Bogus.DataSets.Name.Gender.Male : Bogus.DataSets.Name.Gender.Female),
                LastName = faker.Name.LastName(),
                BirthDate = DateOnly.FromDateTime(faker.Date.Between(new DateTime(2000, 1, 1), new DateTime(2012, 12, 31))),
                Gender = gender,
                Club = rnd.Bool(0.9f) ? rnd.ListItem(clubs) : null
            };
            swimmers.Add(swimmer);

            var ability = rnd.Double(1.08, 1.45); // cât de lent e față de baza WA
            var swimmerEvents = rnd.Shuffle(events.Where(e => e.Event.Gender == gender)).Take(rnd.Int(4, 6));

            foreach (var (ev, baseSec) in swimmerEvents)
            {
                var chosenMeets = rnd.Shuffle(meets).Take(rnd.Int(3, 6)).OrderBy(m => m.Date).ToList();

                for (var m = 0; m < chosenMeets.Count; m++)
                {
                    var meet = chosenMeets[m];
                    var poolFactor = meet.PoolLength == PoolLength.Short ? ShortCourseFactor : 1.0;
                    var improvement = 1 + 0.04 * (chosenMeets.Count - 1 - m) / chosenMeets.Count; // cele mai vechi sunt mai lente
                    var noise = rnd.Double(-0.02, 0.03);

                    var timeMs = (int)(baseSec * 1000 * poolFactor * (ability + noise) * improvement);

                    var result = new Result { Swimmer = swimmer, Event = ev, Meet = meet, TimeMs = timeMs };

                    if (ev.DistanceMeters >= 100)
                        foreach (var split in BuildSplits(ev.DistanceMeters, timeMs, rnd))
                            result.Splits.Add(split);

                    results.Add(result);
                }
            }
        }

        db.AddRange(events.Select(e => e.Event));
        db.AddRange(clubs);
        db.AddRange(meets);
        db.AddRange(swimmers);
        db.AddRange(results);

        await db.SaveChangesAsync();
    }

    // splits la fiecare 50m (fără ultimul), cu o tendință de a pleca prea tare sau de a termina tare
    private static List<Split> BuildSplits(int distance, int totalMs, Randomizer rnd)
    {
        var segments = distance / 50;
        var fade = rnd.Double(-0.06, 0.12); // >0 = a doua jumătate mai lentă

        var weights = Enumerable.Range(0, segments)
            .Select(i => 1 + fade * (i - (segments - 1) / 2.0) / segments)
            .ToArray();
        var sum = weights.Sum();

        var splits = new List<Split>();
        double cumulative = 0;
        for (var i = 0; i < segments - 1; i++)
        {
            cumulative += weights[i];
            splits.Add(new Split
            {
                DistanceMeters = (i + 1) * 50,
                CumulativeTimeMs = (int)Math.Round(totalMs * cumulative / sum)
            });
        }
        return splits;
    }
}