using GolfBrandSim.Core.Enums;

namespace GolfBrandSim.Core.Domain;

public sealed class Tournament
{
    public Tournament(int weekNumber, string name, string venueName, TournamentType type, decimal purse, int fieldSize, CourseProfile courseProfile)
    {
        WeekNumber = weekNumber;
        Name = name;
        VenueName = venueName;
        Type = type;
        Purse = purse;
        FieldSize = fieldSize;
        CourseProfile = courseProfile;
    }

    public int WeekNumber { get; }

    public string Name { get; }

    public string VenueName { get; }

    public TournamentType Type { get; }

    public decimal Purse { get; }

    public int FieldSize { get; }

    public CourseProfile CourseProfile { get; }

    public bool IsMajor => Type == TournamentType.Major;
}