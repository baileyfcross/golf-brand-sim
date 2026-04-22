using GolfBrandSim.Core.Domain;

namespace GolfBrandSim.Core.Simulation;

public interface ISeasonGenerator
{
    SeasonSchedule Generate(int year, int fieldSize);
}