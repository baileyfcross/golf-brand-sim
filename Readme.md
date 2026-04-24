# Golf Brand Simulator

Golf Brand Simulator is a C# management game where the player runs a new golf brand, sponsors professional golfers, earns money from tournament performance, and grows the company through products, research, and sponsorship strategy.

The game is built around a PGA-style season with weekly tournaments, major championships, cuts, prize pools, golfer performance, sponsorship contracts, and brand finances.

## Current Status

This project is currently an early playable prototype. The foundation is in place, including season simulation, tournament results, brand finances, sponsorship income, product revenue, and basic UI screens.

The main long-term goal is to turn this into a deeper golf brand management simulator where the player makes meaningful decisions about:

- Which golfers to sponsor
- What contracts to offer
- What products to release
- How to spend money on research and marketing
- How to grow the brand over multiple seasons

## Features Implemented

- 36-week golf season
- 4 major tournaments
- Weekly tournament simulation
- Four-round tournament structure
- Tournament cuts
- Prize pools and payout logic
- Sponsored golfer winnings share
- Brand cash balance
- Finance ledger
- Product categories
- Product revenue
- Research tracks
- Dashboard screen
- Golfer screen
- Calendar screen
- Tournament results screen
- Finance screen
- Basic save/load support
- Unit tests for core simulation behavior

## Planned Features

The next major features are focused on adding more player agency.

### Sponsorship Market

- Available golfer market
- Contract offer creation
- Signing bonuses
- Weekly retainers
- Winnings share negotiation
- Contract length
- Golfer acceptance/rejection logic
- Sponsored golfer list
- Contract expiration and renewal

### Tour Standings

- Season-long golfer standings
- Points list
- Money list
- Wins
- Major wins
- Top tens
- Cuts made
- Best finish
- Tournament history

### Product Launch System

- Player-created product launches
- Apparel, accessories, and equipment
- Launch costs
- Product quality
- Product popularity
- Pricing tiers
- Sponsored golfer sales boosts
- Brand specialization bonuses

### Improved Tournament Simulation

- More golfer attributes
- Driving, approach, short game, putting, mental, form, and stamina
- Course profiles
- Major championship pressure
- Better cut logic
- Fuller payout table
- More realistic tournament outcomes

### New Game Setup

- Brand name selection
- Starting specialization
- Difficulty selection
- Starting cash differences
- Difficulty-based costs and expenses

### Competitor Brands

- AI-controlled brands
- Competitor sponsorships
- Market pressure for high-value golfers
- Golfers signed by other brands

## Project Structure

```text
golf-brand-sim/
├── src/
│   ├── GolfBrandSim.Core/
│   │   ├── Domain/
│   │   ├── Enums/
│   │   └── Simulation/
│   │
│   ├── GolfBrandSim.Game/
│   │   ├── App/
│   │   ├── Screens/
│   │   └── Program.cs
│   │
│   └── GolfBrandSim.Infrastructure/
│       └── Seed/
│
├── tests/
│   └── GolfBrandSim.Tests/
│
└── GolfBrandSim.sln
```

## Architecture

The project is split into three main layers.

### `GolfBrandSim.Core`

Contains the main game logic and domain models.

This layer should include:

- Golfers
- Brands
- Tournaments
- Sponsorship contracts
- Products
- Research
- Finance models
- Simulation services
- Business rules

Core should stay independent from UI code.

### `GolfBrandSim.Infrastructure`

Contains data setup and persistence-related code.

This layer currently handles seed data and should also contain save/load infrastructure as the project grows.

### `GolfBrandSim.Game`

Contains the playable game application and UI screens.

This layer should handle:

- Screen navigation
- Input
- Rendering
- User-facing game flow

Business rules should not be placed directly in UI screens when they can live in Core services.

### `GolfBrandSim.Tests`

Contains unit tests for core game behavior.

Tests should focus on deterministic simulation logic, finance calculations, sponsorship contracts, tournament outcomes, and save/load behavior.

## Requirements

- .NET 9 SDK
- A C# editor such as Visual Studio, Visual Studio Code, or JetBrains Rider

## Getting Started

Clone the repository:

```bash
git clone https://github.com/baileyfcross/golf-brand-sim.git
cd golf-brand-sim
```

Restore dependencies:

```bash
dotnet restore
```

Build the solution:

```bash
dotnet build
```

Run the game:

```bash
dotnet run --project src/GolfBrandSim.Game
```

Run tests:

```bash
dotnet test
```

## Development Commands

Build everything:

```bash
dotnet build
```

Run the game:

```bash
dotnet run --project src/GolfBrandSim.Game
```

Run all tests:

```bash
dotnet test
```

Clean build artifacts:

```bash
dotnet clean
```

## Gameplay Overview

The player controls a golf brand.

Each week, the game advances through the season. Tournaments are simulated, golfers compete for prize money, cuts are applied, and sponsored golfers can generate income for the player's brand based on their contracts.

The brand also earns money through product sales and spends money through operating expenses, research, and sponsorship costs.

The goal is to grow the brand by making smart sponsorship and business decisions over the course of the season.

## Current Gameplay Loop

1. View the dashboard.
2. Check upcoming tournaments.
3. Review sponsored golfers.
4. Advance the week.
5. Watch tournament results.
6. Earn sponsorship income from golfer winnings.
7. Earn product revenue.
8. Pay expenses.
9. Review finances and progress.

## Design Goals

Golf Brand Simulator is designed to be a lightweight sports management game with a focus on brand strategy instead of directly controlling golfers.

The main design goals are:

- Keep the simulation easy to understand.
- Make sponsorship decisions meaningful.
- Let tournament results affect brand growth.
- Build long-term golfer and brand progression.
- Keep the architecture clean and testable.
- Avoid unnecessary complexity early in development.

## Roadmap

### Short Term

- Add real sponsorship contract negotiation
- Add season-long tour standings
- Improve save/load to store full game state
- Add product launch decisions
- Expand unit test coverage

### Medium Term

- Add golfer development and career progression
- Add competitor brands
- Add brand reputation
- Add better product sales modeling
- Improve tournament realism

### Long Term

- Add multi-season progression
- Add rookies and retirements
- Add historical records
- Add brand customization
- Add deeper economy and marketing systems
- Add more polished UI and presentation

## Testing Philosophy

Most game logic should be testable without launching the UI.

Important systems to test include:

- Tournament simulation
- Cut logic
- Prize payouts
- Sponsorship income
- Weekly expenses
- Product revenue
- Research progression
- Save/load behavior
- Season standings
- Contract negotiation

Random simulation behavior should be deterministic when using the same seed so that tests remain stable.

## Contributing Notes

When adding new features:

- Put business logic in `GolfBrandSim.Core`.
- Keep UI code in `GolfBrandSim.Game`.
- Keep seed/save/persistence code in `GolfBrandSim.Infrastructure`.
- Add tests for new simulation or finance logic.
- Avoid putting important game rules directly into screen classes.
- Keep systems simple before making them realistic.
- Make sure `dotnet build` and `dotnet test` pass before committing.

## License

No license has been added yet.
