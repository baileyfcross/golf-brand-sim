using System.Text.Json;
using GolfBrandSim.Core.Domain;
using GolfBrandSim.Core.Enums;
using GolfBrandSim.Core.Finance;
using GolfBrandSim.Core.Simulation;
using GolfBrandSim.Infrastructure.Save;
using GolfBrandSim.Infrastructure.Seed;

namespace GolfBrandSim.Game.App;

public sealed class SaveGameStore
{
    private readonly string _saveFilePath;

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        WriteIndented = false
    };

    public SaveGameStore(string? saveFilePath = null)
    {
        _saveFilePath = saveFilePath ?? BuildDefaultPath();
    }

    public bool HasSaveFile()
    {
        return File.Exists(_saveFilePath);
    }

    public void Save(GameSession session)
    {
        var state = session.State;
        var data = new SaveGameData
        {
            SeasonYear = state.SeasonSchedule.Year,
            CurrentWeekNumber = state.CurrentWeekNumber,
            Brand = SerializeBrand(state.PlayerBrand),
            Golfers = state.Golfers.Select(SerializeGolfer).ToList(),
            Tournaments = state.SeasonSchedule.Tournaments.Select(SerializeTournament).ToList(),
            SeasonStandings = SerializeStandings(state.SeasonStandings),
            FinanceLedger = state.FinanceLedger.Entries.Select(SerializeFinanceEntry).ToList(),
            LastWeekResult = state.LastWeekResult is null ? null : SerializeLastWeek(state.LastWeekResult)
        };

        Directory.CreateDirectory(Path.GetDirectoryName(_saveFilePath)!);
        var json = JsonSerializer.Serialize(data, JsonOptions);
        File.WriteAllText(_saveFilePath, json);
    }

    public bool TryLoad(out GameSession session)
    {
        session = null!;

        if (!File.Exists(_saveFilePath))
            return false;

        try
        {
            var json = File.ReadAllText(_saveFilePath);
            var data = JsonSerializer.Deserialize<SaveGameData>(json, JsonOptions);
            if (data is null)
                return false;

            session = DeserializeSession(data);
            return true;
        }
        catch
        {
            return false;
        }
    }

    private static GameSession DeserializeSession(SaveGameData data)
    {
        var golfers = data.Golfers.Select(DeserializeGolfer).ToList();
        var golferMap = golfers.ToDictionary(g => g.Id);

        var brand = DeserializeBrand(data.Brand);
        var financeLedger = new FinanceLedger();
        foreach (var entry in data.FinanceLedger)
            financeLedger.Add(DeserializeFinanceEntry(entry));

        var tournaments = data.Tournaments.Select(DeserializeTournament).ToList();
        var schedule = new SeasonSchedule(data.SeasonYear, tournaments);

        var state = new GameState(brand, golfers, schedule, financeLedger);

        // Restore standings
        RestoreStandings(state.SeasonStandings, data.SeasonStandings);

        // Rebuild last week result if available
        WeekSimulationResult? lastWeekResult = null;
        if (data.LastWeekResult is not null)
        {
            var tournData = data.Tournaments.FirstOrDefault(t => t.WeekNumber == data.LastWeekResult.WeekNumber);
            if (tournData is not null)
                lastWeekResult = DeserializeLastWeek(data.LastWeekResult, golferMap, tournData);
        }

        // Fast-forward the week counter using placeholder week results so CurrentWeekNumber is correct
        for (var i = 1; i < data.CurrentWeekNumber; i++)
        {
            var isLast = i == data.CurrentWeekNumber - 1 && lastWeekResult is not null;
            state.RecordWeek(isLast ? lastWeekResult! : CreatePlaceholderWeekResult(i));
        }

        ITournamentSimulator simulator = new TournamentSimulator(Environment.TickCount);
        return new GameSession(state, new WeeklyGameLoop(simulator));
    }

    private static void RestoreStandings(SeasonStandings standings, StandingsSaveData data)
    {
        foreach (var (idStr, statsData) in data.Stats)
        {
            if (Guid.TryParse(idStr, out var id))
                standings.RestoreStats(id, statsData.Points, statsData.Wins, statsData.Top10s, statsData.Earnings);
        }
    }

    private static WeekSimulationResult CreatePlaceholderWeekResult(int weekNumber)
    {
        return new WeekSimulationResult(weekNumber, null!, 0m, 0m, 0m, 0m, 0m, []);
    }

    private static WeekSimulationResult DeserializeLastWeek(LastWeekSaveData data, Dictionary<Guid, Golfer> golferMap, TournamentSaveData tournData)
    {
        var tournament = DeserializeTournament(tournData);
        var standings = data.TournamentResult?.Standings
            .Where(s => golferMap.ContainsKey(s.GolferId))
            .Select(s => new TournamentStanding(golferMap[s.GolferId], s.Place, s.RoundScores, s.MadeCut, s.PrizeMoney))
            .ToList() ?? [];
        var tournResult = new TournamentResult(tournament, standings, data.TournamentResult?.CutScore ?? 0);

        return new WeekSimulationResult(
            data.WeekNumber,
            tournResult,
            data.ProductProfit,
            data.SponsorshipIncome,
            data.OperatingExpense,
            data.NetCashChange,
            data.EndingCashBalance,
            []);
    }

    private static BrandSaveData SerializeBrand(Brand brand)
    {
        return new BrandSaveData
        {
            Id = brand.Id,
            Name = brand.Name,
            Specialization = brand.Specialization,
            CashBalance = brand.CashBalance,
            Products = brand.Products.Select(p => new ProductSaveData
            {
                Name = p.Name,
                Category = p.Category,
                WeeklyRevenueBase = p.WeeklyRevenueBase,
                MarginRate = p.MarginRate,
                IsActive = p.IsActive,
                QualityTier = p.QualityTier
            }).ToList(),
            ResearchTracks = brand.ResearchTracks.Select(r => new ResearchSaveData
            {
                Category = r.Category,
                ProgressPoints = r.ProgressPoints,
                UnlockThreshold = r.UnlockThreshold,
                IsUnlocked = r.IsUnlocked
            }).ToList(),
            Contracts = brand.Contracts.Select(c => new ContractSaveData
            {
                GolferId = c.GolferId,
                ContractName = c.ContractName,
                WinningsShareRate = c.WinningsShareRate,
                AnnualRetainer = c.AnnualRetainer,
                StartWeek = c.StartWeek,
                EndWeek = c.EndWeek
            }).ToList()
        };
    }

    private static Brand DeserializeBrand(BrandSaveData data)
    {
        var products = data.Products.Select(p =>
            new BrandProduct(p.Name, p.Category, p.WeeklyRevenueBase, p.MarginRate, p.IsActive, p.QualityTier));

        var research = data.ResearchTracks.Select(r =>
        {
            var track = new ResearchState(r.Category, r.UnlockThreshold, r.IsUnlocked);
            if (!r.IsUnlocked && r.ProgressPoints > 0)
                track.Advance(r.ProgressPoints);
            return track;
        });

        var contracts = data.Contracts.Select(c =>
            new SponsorshipContract(c.GolferId, c.ContractName, c.WinningsShareRate, c.AnnualRetainer, c.StartWeek, c.EndWeek));

        return new Brand(data.Id, data.Name, data.Specialization, data.CashBalance, products, research, contracts);
    }

    private static GolferSaveData SerializeGolfer(Golfer g) => new()
    {
        Id = g.Id, FullName = g.FullName, CountryCode = g.CountryCode, Age = g.Age,
        DrivingDistance = g.DrivingDistance, DrivingAccuracy = g.DrivingAccuracy,
        Approach = g.Approach, ShortGame = g.ShortGame, Putting = g.Putting,
        Mentality = g.Mentality, Consistency = g.Consistency,
        Popularity = g.Popularity, Marketability = g.Marketability
    };

    private static Golfer DeserializeGolfer(GolferSaveData d) =>
        new(d.Id, d.FullName, d.CountryCode, d.Age, d.DrivingDistance, d.DrivingAccuracy,
            d.Approach, d.ShortGame, d.Putting, d.Mentality, d.Consistency, d.Popularity, d.Marketability);

    private static TournamentSaveData SerializeTournament(Tournament t) => new()
    {
        WeekNumber = t.WeekNumber, Name = t.Name, VenueName = t.VenueName,
        Type = t.Type, Purse = t.Purse, FieldSize = t.FieldSize,
        CourseProfile = new CourseProfileSaveData
        {
            DistanceDemand = t.CourseProfile.DistanceDemand,
            AccuracyDemand = t.CourseProfile.AccuracyDemand,
            ApproachDemand = t.CourseProfile.ApproachDemand,
            ShortGameDemand = t.CourseProfile.ShortGameDemand,
            PuttingDemand = t.CourseProfile.PuttingDemand,
            Difficulty = t.CourseProfile.Difficulty,
            Volatility = t.CourseProfile.Volatility
        }
    };

    private static Tournament DeserializeTournament(TournamentSaveData d)
    {
        var profile = new CourseProfile(d.CourseProfile.DistanceDemand, d.CourseProfile.AccuracyDemand,
            d.CourseProfile.ApproachDemand, d.CourseProfile.ShortGameDemand, d.CourseProfile.PuttingDemand,
            d.CourseProfile.Difficulty, d.CourseProfile.Volatility);
        return new Tournament(d.WeekNumber, d.Name, d.VenueName, d.Type, d.Purse, d.FieldSize, profile);
    }

    private static StandingsSaveData SerializeStandings(SeasonStandings standings)
    {
        var data = new StandingsSaveData();
        foreach (var (id, stats) in standings.Stats)
        {
            data.Stats[id.ToString()] = new GolferStatsSaveData
            {
                Points = stats.Points, Wins = stats.Wins, Top10s = stats.Top10s, Earnings = stats.Earnings
            };
        }
        return data;
    }

    private static FinanceEntrySaveData SerializeFinanceEntry(FinanceEntry e) => new()
    {
        WeekNumber = e.WeekNumber, Type = e.Type.ToString(),
        Description = e.Description, Amount = e.Amount
    };

    private static FinanceEntry DeserializeFinanceEntry(FinanceEntrySaveData d)
    {
        Enum.TryParse<FinanceEntryType>(d.Type, out var type);
        return new FinanceEntry(d.WeekNumber, type, d.Description, d.Amount);
    }

    private static LastWeekSaveData SerializeLastWeek(WeekSimulationResult result)
    {
        return new LastWeekSaveData
        {
            WeekNumber = result.WeekNumber,
            ProductProfit = result.ProductProfit,
            SponsorshipIncome = result.SponsorshipIncome,
            OperatingExpense = result.OperatingExpense,
            NetCashChange = result.NetCashChange,
            EndingCashBalance = result.EndingCashBalance,
            TournamentResult = new TournamentResultSaveData
            {
                TournamentName = result.TournamentResult.Tournament.Name,
                CutScore = result.TournamentResult.CutScore,
                Standings = result.TournamentResult.Standings.Select(s => new StandingSaveData
                {
                    GolferId = s.Golfer.Id,
                    Place = s.Place,
                    RoundScores = s.RoundScores.ToList(),
                    MadeCut = s.MadeCut,
                    PrizeMoney = s.PrizeMoney
                }).ToList()
            }
        };
    }

    private static string BuildDefaultPath()
    {
        var root = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        return Path.Combine(root, "GolfBrandSim", "save-slot-1.json");
    }
}
