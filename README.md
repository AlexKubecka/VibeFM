# Football Manager Simulation

## Overview
This project simulates real-world football leagues, teams, and players using a modern ASP.NET Core MVC web application with a SQLite database backend. You can view league tables, team and player details, and simulate an entire season with a single click.

---

## Features
- ASP.NET Core MVC web UI for browsing leagues, teams, and players
- Real football data seeded from JSON (teams, players, staff, market values, stadiums)
- League table with live stats (Wins, Draws, Losses, Goals For/Against, Points, etc.)
- Simulate a full season for any league with one click
- Most valuable players leaderboard
- Entity Framework Core with SQLite database
- Debug mode for detailed logging
- Unit tests for core logic

---

## How to Run
1. Clone the repository:
   ```bash
   git clone https://github.com/yourusername/VibeFM.git
   cd VibeFM
   ```
2. Build and run the web application:
   ```bash
   dotnet build FootballManager.Web/FootballManager.Web.csproj
   dotnet run --project FootballManager.Web/FootballManager.Web.csproj
   ```
3. Open your browser and go to `https://localhost:5001` (or the URL shown in the console).

---

## Usage
- The database is seeded with real football data on startup.
- Navigate to the "Leagues" tab to view all leagues and their tables.
- Click on a league to see the league table and most valuable players.
- Click the "Simulate Season" button to randomly simulate all matches in the league and update the table.
- Click on any team or player for detailed stats.

---

## TODO List

### Core Features
- [x] Web UI for leagues, teams, and players
- [x] League table with real stats and simulation
- [x] Database seeding from real data
- [x] Player and staff factories

### Enhancements
- [ ] Player transfers between teams
- [ ] Injuries and morale
- [ ] Expand staff roles (e.g., Fitness Coach, Scout)
- [ ] Team tactics and formations

### Testing
- [x] Unit tests for Player and Team
- [ ] Unit tests for League simulation
- [ ] Integration tests for factories

### Documentation
- [x] README with project overview and features
- [ ] Add detailed comments to all methods and classes
- [ ] Create a user guide for running the simulation and interpreting results