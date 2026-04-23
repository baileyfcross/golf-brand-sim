using GolfBrandSim.Core.Domain;

namespace GolfBrandSim.Infrastructure.Seed;

public static class FictionalGolferSeedData
{
    // Fields: fullName, cc, age, drivingDistance, drivingAccuracy, approach, shortGame, putting, mentality, consistency, popularity, marketability
    // Overall = DD*0.10 + DA*0.15 + AP*0.25 + SG*0.20 + PT*0.20 + ME*0.05 + CO*0.05
    // Archetypes: B=Bomber, A=Accurate, I=Iron, S=ShortGame, P=Putting, Ba=Balanced

    public static IReadOnlyList<Golfer> CreateGolfers()
    {
        static Golfer G(string n, string cc, int age, int dd, int da, int ap, int sg, int pt, int me, int co, int pop, int mkt)
            => new(Guid.NewGuid(), n, cc, age, dd, da, ap, sg, pt, me, co, pop, mkt);

        return
        [
            // --- ELITE TIER (Overall 88-95) ---
            G("Liam Hart",        "USA", 29, 88, 83, 91, 88, 89, 90, 85, 91, 92), // Ba
            G("Mateo Alvarez",    "ESP", 31, 77, 89, 94, 89, 87, 86, 83, 84, 85), // I
            G("Riku Sato",        "JPN", 27, 79, 81, 89, 94, 93, 85, 84, 73, 74), // S
            G("Theo Mercer",      "ENG", 33, 96, 73, 85, 81, 83, 88, 79, 82, 83), // B
            G("Jae-Won Yoon",     "KOR", 26, 81, 83, 88, 89, 95, 87, 88, 76, 78), // P
            G("Carlos Mendoza",   "COL", 34, 74, 94, 92, 86, 85, 84, 90, 79, 80), // A

            // --- TOUR STARS (Overall 82-87) ---
            G("Ethan Cross",      "USA", 28, 85, 80, 84, 82, 81, 83, 81, 75, 74), // Ba
            G("Noah Bennett",     "CAN", 30, 73, 87, 83, 79, 80, 79, 80, 71, 70), // A
            G("Owen Gallagher",   "IRL", 35, 75, 78, 83, 88, 86, 81, 86, 66, 65), // S
            G("Julian Price",     "USA", 25, 82, 77, 81, 79, 78, 80, 82, 69, 68), // Ba
            G("Logan Frost",      "AUS", 32, 92, 69, 80, 74, 75, 77, 75, 64, 63), // B
            G("Dylan Rhodes",     "USA", 29, 76, 75, 80, 81, 90, 79, 75, 68, 67), // P
            G("Adrian Vale",      "RSA", 36, 75, 83, 87, 78, 77, 80, 73, 72, 71), // I
            G("Gabriel Stone",    "USA", 24, 80, 76, 79, 82, 81, 76, 85, 61, 60), // Ba
            G("Hugo Laurent",     "FRA", 31, 71, 86, 83, 77, 76, 78, 71, 67, 66), // A
            G("Nico Barlow",      "SCO", 33, 78, 80, 81, 77, 77, 80, 80, 63, 62), // Ba
            G("Felix Wagner",     "GER", 28, 89, 67, 77, 73, 74, 75, 74, 62, 61), // B
            G("Kei Nakamura",     "JPN", 29, 77, 77, 79, 88, 84, 76, 70, 65, 64), // S
            G("Rafael Costa",     "BRA", 27, 80, 73, 78, 78, 77, 77, 72, 60, 59), // Ba
            G("Marcus Webb",      "USA", 32, 75, 74, 78, 80, 88, 78, 73, 63, 62), // P

            // --- SOLID TOUR (Overall 75-81) ---
            G("Carson Pike",      "USA", 26, 76, 71, 75, 74, 73, 75, 77, 55, 54), // Ba
            G("Benji Ford",       "AUS", 23, 87, 63, 71, 67, 68, 69, 82, 56, 55), // B
            G("Parker Shaw",      "NZL", 29, 68, 82, 78, 73, 72, 74, 76, 57, 56), // A
            G("Micah Cole",       "USA", 30, 71, 73, 75, 84, 81, 73, 78, 58, 57), // S
            G("Mason Clarke",     "USA", 25, 75, 71, 74, 73, 72, 73, 73, 52, 51), // Ba
            G("Jonas Meyer",      "SWE", 28, 66, 84, 77, 71, 71, 75, 79, 50, 49), // A
            G("Ty Brooks",        "USA", 22, 84, 61, 69, 65, 66, 67, 75, 48, 47), // B
            G("Kyle Novak",       "USA", 31, 73, 74, 74, 72, 73, 74, 74, 54, 53), // Ba
            G("Javier Ruiz",      "MEX", 28, 71, 77, 81, 73, 71, 71, 70, 56, 55), // I
            G("Anders Lindqvist", "SWE", 27, 65, 83, 77, 69, 70, 73, 77, 51, 50), // A
            G("Declan Murray",    "IRL", 34, 69, 71, 73, 83, 79, 72, 75, 53, 52), // S
            G("Patrick Dumont",   "FRA", 29, 72, 72, 73, 71, 71, 73, 71, 55, 54), // Ba
            G("Kai Yamamoto",     "JPN", 26, 70, 79, 80, 71, 69, 72, 69, 52, 51), // I
            G("Santiago Vega",    "CHI", 30, 74, 69, 72, 70, 71, 71, 70, 49, 48), // Ba
            G("Brendan Frost",    "AUS", 27, 72, 72, 73, 70, 71, 72, 73, 53, 52), // Ba
            G("Tyler Jackson",    "USA", 24, 85, 58, 68, 63, 64, 66, 73, 47, 46), // B
            G("Samuel Grant",     "ENG", 33, 64, 81, 75, 68, 68, 72, 75, 52, 51), // A
            G("Oscar Nilsson",    "SWE", 28, 71, 73, 73, 70, 71, 72, 72, 51, 50), // Ba
            G("Rodrigo Pena",     "ARG", 31, 70, 71, 72, 70, 70, 70, 71, 50, 49), // Ba
            G("Finn Gallagher",   "IRL", 25, 67, 69, 72, 74, 84, 73, 74, 49, 48), // P
            G("Leon Hartmann",    "GER", 30, 68, 77, 79, 69, 68, 70, 68, 51, 50), // I
            G("Pierre Bonnet",    "FRA", 27, 67, 69, 72, 82, 77, 71, 73, 52, 51), // S
            G("Dong-Hyun Park",   "KOR", 23, 64, 83, 76, 67, 67, 71, 77, 48, 47), // A
            G("Chris Holden",     "USA", 35, 70, 71, 71, 70, 70, 70, 72, 53, 52), // Ba

            // --- MID TOUR (Overall 67-74) ---
            G("Blake Morrison",   "USA", 27, 73, 68, 71, 69, 70, 71, 71, 46, 45), // Ba
            G("Hiroshi Tanaka",   "JPN", 31, 65, 76, 73, 68, 67, 69, 67, 47, 46), // I
            G("Marco Ferretti",   "ITA", 29, 68, 70, 71, 72, 70, 70, 70, 48, 47), // Ba
            G("Sean Callahan",    "USA", 26, 70, 67, 70, 68, 69, 69, 72, 44, 43), // Ba
            G("Lucas Petit",      "FRA", 24, 66, 71, 69, 71, 72, 70, 71, 45, 44), // Ba
            G("Yuki Ishida",      "JPN", 22, 68, 74, 74, 69, 68, 71, 69, 46, 45), // I
            G("Ryan Shelton",     "USA", 34, 72, 65, 69, 67, 68, 69, 70, 43, 42), // Ba
            G("Nate Crawford",    "USA", 28, 80, 57, 66, 62, 63, 65, 73, 41, 40), // B
            G("Alvaro Santos",    "ESP", 30, 64, 74, 72, 67, 67, 68, 68, 47, 46), // I
            G("Will Pemberton",   "ENG", 27, 66, 69, 69, 70, 71, 70, 70, 45, 44), // Ba
            G("Dave Tanner",      "USA", 33, 68, 67, 69, 68, 69, 69, 71, 44, 43), // Ba
            G("Florian Kiefer",   "GER", 26, 65, 72, 72, 68, 67, 70, 67, 46, 45), // I
            G("Bruno Matos",      "BRA", 29, 70, 66, 69, 67, 68, 68, 69, 43, 42), // Ba
            G("Connor Dunn",      "IRL", 32, 67, 70, 70, 73, 74, 70, 72, 47, 46), // S
            G("Erik Sundqvist",   "SWE", 28, 64, 73, 71, 67, 67, 68, 69, 45, 44), // A/I
            G("Hwan-Ki Lee",      "KOR", 25, 66, 74, 73, 68, 67, 70, 68, 46, 45), // I
            G("Jordan Mack",      "USA", 31, 72, 66, 69, 68, 68, 69, 70, 43, 42), // Ba
            G("Christoph Braun",  "GER", 27, 63, 72, 72, 67, 67, 69, 67, 44, 43), // I
            G("Tomas Blanco",     "ARG", 24, 70, 65, 68, 67, 68, 68, 70, 41, 40), // Ba
            G("Nathan Wells",     "USA", 36, 67, 69, 70, 69, 70, 70, 72, 46, 45), // Ba
            G("Aaron Holt",       "USA", 29, 69, 67, 69, 68, 68, 69, 70, 43, 42), // Ba
            G("Giulio Conti",     "ITA", 32, 64, 71, 70, 72, 72, 69, 70, 47, 46), // S/P
            G("Daniel Strauss",   "RSA", 27, 65, 69, 70, 68, 68, 69, 68, 44, 43), // Ba
            G("Wei Zhang",        "CHN", 26, 63, 73, 72, 67, 67, 69, 68, 45, 44), // I
            G("Brad Collins",     "USA", 30, 66, 68, 69, 68, 69, 69, 71, 43, 42), // Ba
            G("Sven Holmberg",    "SWE", 34, 63, 74, 71, 67, 66, 68, 68, 44, 43), // A/I
            G("Andy Whitfield",   "AUS", 28, 68, 67, 69, 68, 68, 69, 69, 43, 42), // Ba
            G("Ryuji Mori",       "JPN", 25, 66, 72, 72, 68, 67, 70, 67, 44, 43), // I
            G("Kevin Pryce",      "USA", 33, 67, 68, 70, 68, 69, 69, 70, 43, 42), // Ba
            G("Luca Ferraro",     "ITA", 27, 64, 70, 71, 71, 71, 70, 70, 46, 45), // Ba
            G("Mike Harding",     "USA", 35, 66, 68, 68, 67, 68, 68, 71, 42, 41), // Ba
            G("Callum Rees",      "WAL", 29, 63, 73, 71, 67, 67, 68, 68, 44, 43), // I
            G("Diego Fuentes",    "MEX", 26, 69, 65, 68, 67, 67, 68, 70, 41, 40), // Ba
            G("Peter Van Dyke",   "NED", 31, 64, 70, 70, 71, 71, 69, 70, 45, 44), // Ba
            G("Tomoki Arai",      "JPN", 24, 65, 73, 71, 67, 66, 69, 68, 44, 43), // I
            G("Shane Doherty",    "IRL", 30, 66, 69, 69, 71, 73, 70, 72, 47, 46), // P

            // --- FRINGE TOUR (Overall 55-66) ---
            G("Craig Hammond",    "USA", 32, 64, 64, 66, 65, 65, 66, 67, 38, 37), // Ba
            G("Tony Salinas",     "MEX", 27, 70, 59, 63, 62, 62, 63, 66, 35, 34), // B
            G("Rory Cassidy",     "IRL", 29, 62, 65, 65, 66, 66, 65, 66, 36, 35), // Ba
            G("Scott Burrows",    "USA", 34, 63, 64, 64, 64, 65, 65, 67, 37, 36), // Ba
            G("Nils Osterberg",   "SWE", 25, 60, 68, 66, 63, 63, 64, 65, 37, 36), // I
            G("Claudio Bassi",    "ITA", 31, 61, 64, 65, 66, 67, 64, 66, 37, 36), // Ba
            G("Greg Paxton",      "USA", 26, 64, 63, 64, 63, 64, 64, 65, 35, 34), // Ba
            G("Takeru Osada",     "JPN", 28, 61, 66, 66, 64, 63, 64, 64, 36, 35), // I
            G("Mike Connors",     "USA", 35, 62, 63, 64, 64, 64, 64, 66, 36, 35), // Ba
            G("Lars Pedersen",    "DEN", 30, 59, 68, 65, 62, 62, 63, 65, 36, 35), // I
            G("Ed Hartley",       "ENG", 27, 62, 63, 63, 63, 64, 63, 64, 35, 34), // Ba
            G("Brent Hollis",     "USA", 33, 63, 62, 63, 63, 63, 63, 65, 34, 33), // Ba
            G("Jerome Petit",     "FRA", 29, 60, 65, 64, 65, 65, 64, 65, 36, 35), // Ba
            G("Ian Forsythe",     "SCO", 31, 61, 65, 64, 64, 64, 64, 65, 35, 34), // Ba
            G("George Poole",     "USA", 24, 63, 62, 63, 62, 63, 62, 66, 33, 32), // Ba
            G("Mike Estrada",     "USA", 28, 68, 57, 61, 60, 60, 61, 63, 32, 31), // B
            G("Ren Kawano",       "JPN", 26, 60, 65, 64, 64, 64, 63, 64, 34, 33), // I
            G("Phil Duncan",      "USA", 37, 61, 63, 63, 64, 64, 63, 65, 38, 37), // Ba (vet)
            G("Bruno Cortes",     "BRA", 25, 63, 61, 62, 61, 62, 62, 63, 31, 30), // Ba
            G("Ian McAllister",   "SCO", 32, 60, 64, 63, 64, 64, 63, 65, 35, 34), // Ba
            G("Cody Larsen",      "USA", 22, 65, 58, 62, 60, 60, 61, 64, 31, 30), // B
            G("Alfredo Rios",     "ESP", 28, 59, 64, 63, 63, 63, 63, 63, 33, 32), // Ba
            G("Tom Watkins",      "ENG", 30, 60, 63, 63, 64, 63, 63, 64, 35, 34), // Ba
            G("Jay Holbrook",     "USA", 26, 62, 61, 62, 62, 62, 62, 63, 32, 31), // Ba
            G("Franco Ricci",     "ITA", 29, 59, 63, 63, 64, 64, 63, 64, 34, 33), // Ba
            G("Mark Svensson",    "SWE", 27, 58, 65, 63, 62, 62, 62, 64, 33, 32), // A/I
            G("Sam Whitaker",     "USA", 31, 61, 62, 62, 62, 63, 62, 63, 33, 32), // Ba
            G("T.J. Rollins",     "USA", 24, 64, 58, 61, 60, 60, 60, 63, 30, 29), // B
            G("Marco Rueda",      "COL", 28, 59, 63, 62, 62, 62, 62, 62, 32, 31), // Ba
            G("Yuji Watanabe",    "JPN", 25, 59, 65, 63, 63, 62, 62, 63, 33, 32), // I
            G("Finn Eriksson",    "NOR", 27, 58, 64, 62, 62, 62, 62, 63, 31, 30), // Ba
            G("Ben Gallagher",    "USA", 23, 60, 61, 61, 61, 61, 61, 63, 30, 29), // Ba
            G("Pieter Smits",     "NED", 30, 57, 64, 62, 62, 62, 62, 63, 31, 30), // Ba
            G("Tao Lin",          "CHN", 26, 58, 65, 63, 62, 62, 62, 62, 33, 32), // I
            G("Curtis Nolan",     "USA", 29, 60, 61, 61, 62, 61, 61, 62, 31, 30), // Ba
            G("Henrik Soberg",    "NOR", 32, 57, 64, 62, 62, 62, 62, 63, 30, 29), // Ba
            G("Danny Cho",        "KOR", 24, 58, 64, 62, 62, 62, 62, 62, 32, 31), // Ba
            G("Will Barker",      "ENG", 28, 59, 62, 62, 62, 62, 62, 63, 32, 31), // Ba
            G("Cole Davenport",   "USA", 22, 61, 59, 60, 60, 60, 60, 62, 28, 27), // Ba
            G("Jin Ho Choi",      "KOR", 25, 58, 64, 62, 62, 62, 62, 62, 33, 32), // Ba
        ];
    }
}
