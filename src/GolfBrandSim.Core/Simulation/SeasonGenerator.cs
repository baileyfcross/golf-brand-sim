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
                tournaments.Add(new Tournament(week, major.Name, major.Venue, TournamentType.Major, 14_000_000m, fieldSize));
                continue;
            }

            var standardEvent = StandardEvents[standardEventIndex % StandardEvents.Length];
            tournaments.Add(new Tournament(week, standardEvent.Name, standardEvent.Venue, TournamentType.Standard, 8_200_000m, fieldSize));
            standardEventIndex++;
        }

        return new SeasonSchedule(year, tournaments);
    }
}