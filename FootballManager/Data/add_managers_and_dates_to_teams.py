import json

# Provided manager data
manager_data = {
    "Arsenal FC": ("Mikel Arteta", "22 Dec 2019"),
    "Aston Villa": ("Unai Emery", "5 Nov 2022"),
    "AFC Bournemouth": ("Andoni Iraola", "1 Jul 2023"),
    "Brentford FC": ("Keith Andrews", "27 Jun 2025"),
    "Brighton & Hove Albion": ("Fabian Hurzeler", "15 Jun 2024"),
    "Burnley": ("Scott Parker", "5 July 2024"),
    "Chelsea FC": ("Enzo Maresca", "3 June 2024"),
    "Crystal Palace": ("Oliver Glasner", "20 Feb 2024"),
    "Everton FC": ("David Moyes", "11 Jan 2025"),
    "Fulham FC": ("Marco Silva", "1 Jul 2021"),
    "Leeds United": ("Daniel Farke", "4 Jul 2023"),
    "Liverpool FC": ("Arne Slot", "1 Jun 2024"),
    "Manchester City": ("Pep Guardiola", "1 Jul 2016"),
    "Manchester United": ("Ruben Amorim", "11 Nov 2024"),
    "Newcastle United": ("Eddie Howe", "8 Nov 2021"),
    "Nottingham Forest": ("Nuno Espirito Santo", "20 Dec 2023"),
    "Sunderland": ("Regis Le Bris", "1 Jul 2024"),
    "Tottenham Hotspur": ("Thomas Frank", "12 Jun 2025"),
    "West Ham United": ("Graham Potter", "9 Jan 2025"),
    "Wolverhampton Wanderers": ("Vitor Pereira", "19 Dec 2024")
}

with open('combined_team_data_wrapped.json', 'r', encoding='utf-8') as f:
    data = json.load(f)

teams = data.get('Teams', {})
for team_name, team_obj in teams.items():
    if team_name in manager_data:
        name, start_date = manager_data[team_name]
        team_obj['Manager'] = {"Name": name, "StartDate": start_date}

with open('combined_team_data_wrapped.json', 'w', encoding='utf-8') as f:
    json.dump(data, f, ensure_ascii=False, indent=2)
