import json

# Load the wrapped JSON data
with open('combined_team_data_wrapped.json', 'r', encoding='utf-8') as f:
    data = json.load(f)

league_name = data.get('LeagueName', None)
if not league_name:
    raise ValueError('LeagueName not found at the top level of the JSON file.')
league_nation = data.get('LeagueNation', None)
if not league_nation:
    raise ValueError('LeagueNation not found at the top level of the JSON file.')
manager = data.get('Manager', None)
if not manager:
    raise ValueError('Manager not found at the top level of the JSON file.')

teams = data.get('Teams', {})
for team_name, team_obj in teams.items():
    team_obj['LeagueName'] = league_name
    team_obj['LeagueNation'] = league_nation
    team_obj['Manager'] = manager

# Save the updated JSON data
with open('combined_team_data_wrapped.json', 'w', encoding='utf-8') as f:
    json.dump(data, f, ensure_ascii=False, indent=2)
