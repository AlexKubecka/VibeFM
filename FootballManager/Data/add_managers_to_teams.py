import json
import requests
from bs4 import BeautifulSoup
import time

# Load the wrapped JSON data
with open('combined_team_data_wrapped.json', 'r', encoding='utf-8') as f:
    data = json.load(f)

# You must provide a mapping from team name to Transfermarkt team URL
# Example: {"Manchester City": "https://www.transfermarkt.com/manchester-city/startseite/verein/281"}
# You can build this mapping from your scrape or manually for a temp run
team_url_map = {
    "Manchester City": "https://www.transfermarkt.com/manchester-city/startseite/verein/281/saison_id/2025",
    "Arsenal FC": "https://www.transfermarkt.com/arsenal-fc/startseite/verein/11/saison_id/2025",
    "Chelsea FC": "https://www.transfermarkt.com/chelsea-fc/startseite/verein/631/saison_id/2025",
    "Liverpool FC": "https://www.transfermarkt.com/liverpool-fc/startseite/verein/31/saison_id/2025",
    "Tottenham Hotspur": "https://www.transfermarkt.com/tottenham-hotspur/startseite/verein/148/saison_id/2025",
    "Manchester United": "https://www.transfermarkt.com/manchester-united/startseite/verein/985/saison_id/2025",
    "Newcastle United": "https://www.transfermarkt.com/newcastle-united/startseite/verein/762/saison_id/2025",
    "Brighton & Hove Albion": "https://www.transfermarkt.com/brighton-amp-hove-albion/startseite/verein/1237/saison_id/2025",
    "Aston Villa": "https://www.transfermarkt.com/aston-villa/startseite/verein/405/saison_id/2025",
    "Crystal Palace": "https://www.transfermarkt.com/crystal-palace/startseite/verein/873/saison_id/2025",
    "AFC Bournemouth": "https://www.transfermarkt.com/afc-bournemouth/startseite/verein/989/saison_id/2025",
    "Nottingham Forest": "https://www.transfermarkt.com/nottingham-forest/startseite/verein/703/saison_id/2025",
    "Brentford FC": "https://www.transfermarkt.com/brentford-fc/startseite/verein/1148/saison_id/2025",
    "West Ham United": "https://www.transfermarkt.com/west-ham-united/startseite/verein/379/saison_id/2025",
    "Fulham FC": "https://www.transfermarkt.com/fulham-fc/startseite/verein/931/saison_id/2025",
    "Everton FC": "https://www.transfermarkt.com/everton-fc/startseite/verein/29/saison_id/2025",
    "Wolverhampton Wanderers": "https://www.transfermarkt.com/wolverhampton-wanderers/startseite/verein/543/saison_id/2025",
    "Leicester City": "https://www.transfermarkt.com/leicester-city/startseite/verein/1003/saison_id/2025",
    "Ipswich Town": "https://www.transfermarkt.com/ipswich-town/startseite/verein/677/saison_id/2025",
    "Southampton FC": "https://www.transfermarkt.com/southampton-fc/startseite/verein/180/saison_id/2025"
}

# XPath equivalent for manager: //a[contains(@href, '/profil/trainer/') and contains(@class, 'name')]
def get_manager_name(team_url):
    headers = {
        'User-Agent': 'Mozilla/5.0 (Windows NT 10.0; Win64; x64)'
    }
    resp = requests.get(team_url, headers=headers)
    soup = BeautifulSoup(resp.text, 'html.parser')
    manager_a = soup.find('a', href=lambda x: x and '/profil/trainer/' in x)
    if manager_a:
        return manager_a.get_text(strip=True)
    return None

teams = data.get('Teams', {})
for team_name, team_obj in teams.items():
    team_url = team_url_map.get(team_name)
    if not team_url:
        print(f"No URL for team: {team_name}")
        continue
    print(f"Scraping manager for {team_name}...")
    manager = get_manager_name(team_url)
    if manager:
        team_obj['Manager'] = manager
        print(f"  Found: {manager}")
    else:
        print(f"  Manager not found for {team_name}")
    time.sleep(2)  # Be polite to the server

# Save the updated JSON data
with open('combined_team_data_wrapped.json', 'w', encoding='utf-8') as f:
    json.dump(data, f, ensure_ascii=False, indent=2)
