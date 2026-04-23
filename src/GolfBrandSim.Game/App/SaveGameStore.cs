using System.Text.Json;
using GolfBrandSim.Core.Simulation;
using GolfBrandSim.Infrastructure.Seed;

namespace GolfBrandSim.Game.App;

public sealed class SaveGameStore
{
    private readonly string _saveFilePath;

    public SaveGameStore(string? saveFilePath = null)
    {
        _saveFilePath = saveFilePath ?? BuildDefaultPath();
    }

    public bool HasSaveFile()
    {
        return File.Exists(_saveFilePath);
    }

    public void Save(GameSession session, int seed)
    {
        var payload = new SaveGamePayload(seed, session.State.CurrentWeekNumber);
        Directory.CreateDirectory(Path.GetDirectoryName(_saveFilePath)!);
        var json = JsonSerializer.Serialize(payload);
        File.WriteAllText(_saveFilePath, json);
    }

    public bool TryLoad(out GameSession session, out int seed)
    {
        session = null!;
        seed = 0;

        if (!File.Exists(_saveFilePath))
        {
            return false;
        }

        try
        {
            var json = File.ReadAllText(_saveFilePath);
            var payload = JsonSerializer.Deserialize<SaveGamePayload>(json);
            if (payload is null)
            {
                return false;
            }

            seed = payload.Seed;
            session = InitialGameStateFactory.Create(seed);

            var weeksToAdvance = Math.Max(0, payload.CurrentWeekNumber - 1);
            for (var week = 0; week < weeksToAdvance && session.CanAdvanceWeek; week++)
            {
                session.AdvanceWeek();
            }

            return true;
        }
        catch
        {
            return false;
        }
    }

    private static string BuildDefaultPath()
    {
        var root = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        return Path.Combine(root, "GolfBrandSim", "save-slot-1.json");
    }

    private sealed record SaveGamePayload(int Seed, int CurrentWeekNumber);
}