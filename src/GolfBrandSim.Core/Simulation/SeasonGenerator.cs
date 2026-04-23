using GolfBrandSim.Core.Domain;
using GolfBrandSim.Core.Enums;

namespace GolfBrandSim.Core.Simulation;

public sealed class SeasonGenerator : ISeasonGenerator
{
    private static readonly Dictionary<int, (string Name, string Venue)> MajorWeeks = new()
    {
        [8] = ("MAGNOLIA MASTERS", "AUGUSTA RIDGE"),
        [16] = ("COASTAL PGA", "SEABREAK POINT"),
        [25] = ("ROYAL OPEN", "HARBOR DUNES"),
        [34] = ("SUMMIT CHAMPIONSHIP", "EAGLE CREST CLUB")
    };

    private static readonly (string Name, string Venue)[] StandardEvents =
    [
        ("DESERT CLASSIC", "SILVER MESA"),
        ("PACIFIC INVITATIONAL", "CYPRESS SHORES"),
        ("VALLEY OPEN", "RED ROCK NATIONAL"),
        ("RIVER CITY CHAMPIONSHIP", "FOUR BRIDGES"),
        ("PINE COASTAL", "PINE WATCH"),
        ("LAKELAND INVITATIONAL", "HOLLOW LAKE"),
        ("GREAT PLAINS OPEN", "PRAIRIE CLUB"),
        ("CAPITAL CUP", "FEDERAL LINKS"),
        ("MOUNTAIN CLASSIC", "SUMMIT BLUFFS"),
        ("HEARTLAND OPEN", "MAPLE GLEN"),
        ("SUNBELT INVITATIONAL", "PALMETTO RUN"),
        ("HARBOR CLASSIC", "OCEAN TURN"),
        ("NORTHERN OPEN", "IRONWOOD NATIONAL"),
        ("LAKESIDE CUP", "BLUEWATER DOWNS"),
        ("CROSSWINDS CLASSIC", "WINDMILL HILLS"),
        ("AUTUMN LINKS", "ASH GROVE"),
        ("CHAMPIONS RIDGE", "BLACKSTONE RIDGE"),
        ("TWIN PINES OPEN", "TWIN PINES"),
        ("GOLD COAST INVITATIONAL", "EMERALD BAY"),
        ("CITYLIGHTS CLASSIC", "MIDTOWN COUNTRY CLUB")
    ];

    public SeasonSchedule Generate(int year, int fieldSize)
    {
        var tournaments = new List<Tournament>();
        var standardEventIndex = 0;

        for (var week = 1; week <= 36; week++)
        {
            if (MajorWeeks.TryGetValue(week, out var major))
            {
                var profile = BuildMajorProfile(major.Name);
                tournaments.Add(new Tournament(week, major.Name, major.Venue, TournamentType.Major, 14_000_000m, fieldSize, profile));
                continue;
            }

            var standardEvent = StandardEvents[standardEventIndex % StandardEvents.Length];
            var standardProfile = BuildStandardProfile(standardEvent.Name);
            tournaments.Add(new Tournament(week, standardEvent.Name, standardEvent.Venue, TournamentType.Standard, 8_200_000m, fieldSize, standardProfile));
            standardEventIndex++;
        }

        return new SeasonSchedule(year, tournaments);
    }

    private static CourseProfile BuildMajorProfile(string name) => name switch
    {
        // Augusta-style: approach and short game premium, high difficulty, low volatility
        "MAGNOLIA MASTERS" => new CourseProfile(5, 7, 9, 9, 8, 9, 4),
        // Links-style seaside: accuracy and distance, very high volatility
        "COASTAL PGA" => new CourseProfile(6, 8, 8, 6, 7, 8, 5),
        // Links open: distance and accuracy, high volatility
        "ROYAL OPEN" => new CourseProfile(7, 9, 6, 7, 6, 8, 9),
        // Championship finale: balanced but tough
        "SUMMIT CHAMPIONSHIP" => new CourseProfile(7, 7, 8, 7, 7, 9, 6),
        _ => new CourseProfile(6, 6, 7, 7, 7, 8, 5)
    };

    private static CourseProfile BuildStandardProfile(string name)
    {
        if (name.Contains("DESERT") || name.Contains("VALLEY"))
            return new CourseProfile(8, 6, 7, 5, 6, 5, 6);
        if (name.Contains("COASTAL") || name.Contains("HARBOR") || name.Contains("PACIFIC") || name.Contains("OCEAN"))
            return new CourseProfile(6, 8, 7, 6, 7, 6, 7);
        if (name.Contains("MOUNTAIN") || name.Contains("RIDGE") || name.Contains("SUMMIT"))
            return new CourseProfile(7, 6, 8, 7, 6, 7, 5);
        if (name.Contains("LINKS") || name.Contains("WIND") || name.Contains("CROSS"))
            return new CourseProfile(7, 8, 6, 6, 6, 6, 8);
        if (name.Contains("PINE") || name.Contains("WOOD") || name.Contains("FOREST"))
            return new CourseProfile(6, 7, 7, 7, 6, 6, 5);
        if (name.Contains("LAKE") || name.Contains("WATER") || name.Contains("RIVER"))
            return new CourseProfile(6, 7, 7, 6, 8, 6, 6);
        // Default balanced
        return new CourseProfile(6, 6, 7, 6, 7, 6, 5);
    }
}