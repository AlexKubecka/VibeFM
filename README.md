# Football Manager Simulation

## Overview
This project simulates a football league with teams, players, and staff members. It dynamically generates teams, assigns players and staff, calculates ratings, and ranks teams based on their overall performance.

---

## Features
- Dynamic team creation with unique attributes like reputation, stadium size, and value.
- Position-based player attributes for realistic simulations.
- Job-based staff attributes for managers and coaches.
- League rankings based on calculated overall ratings.
- Debug mode for detailed logging.
- Unit tests to ensure code correctness.

---

## TODO List

### Core Features
- [x] Implement `Team` class with attributes like `Value`, `Reputation`, and `Stadium`.
- [x] Implement `Player` class with position-specific attributes.
- [x] Implement `League` class to manage teams and simulate matches.
- [x] Add factories (`TeamFactory`, `PlayerFactory`, `LeagueFactory`) for dynamic entity creation.

### Enhancements
- [ ] Add match simulation logic to calculate points and generate league standings.
- [ ] Implement player transfers between teams.
- [ ] Add injury and morale attributes for players.
- [ ] Expand staff roles (e.g., Fitness Coach, Scout).
- [ ] Add team tactics and formations to influence match outcomes.

### Testing
- [x] Write unit tests for `Player` class (e.g., `CalculateOverallRating`, `ToString`).
- [x] Write unit tests for `Team` class (e.g., `AddPlayer`, `RemovePlayer`).
- [ ] Write unit tests for `League` class (e.g., `SimulateLeague`, `PrintLeagueTable`).
- [ ] Add integration tests for factories (`TeamFactory`, `PlayerFactory`, `LeagueFactory`).

### Documentation
- [x] Add a README file with project overview and features.
- [ ] Add detailed comments to all methods and classes.
- [ ] Create a user guide for running the simulation and interpreting results.

---

## How to Run
1. Clone the repository:
   ```bash
   git clone https://github.com/yourusername/football-manager.git