# PokeAPI Data Fetcher

This C# console application fetches Pokemon and Move data from PokeAPI for generations 1-5 (Pokemon IDs 1-649).

## Requirements

- .NET 8.0 SDK

## Usage

```bash
cd Server/scripts
dotnet run
```

## Output

- `../../Docs/seeds/pokemons.json` - Contains 649 Pokemon with stats, types, and move lists
- `../../Docs/seeds/moves.json` - Contains all unique moves referenced by the Pokemon

## Features

- Fetches Pokemon data including:
  - Japanese names
  - Base stats (HP, Attack, Defense, Special Attack, Special Defense, Speed)
  - Types (type1 and type2)
  - Evolution levels
  - Move IDs (up to 20 per Pokemon)
  - Front and back sprites
  
- Fetches Move data including:
  - Japanese names
  - Type, Power, Accuracy, PP
  - Priority and Damage Class
  
- Rate limiting: 100ms delay between requests
- Retry logic with 3 attempts for failed requests
- Progress tracking during data fetching

## Notes

- The application respects PokeAPI rate limits
- Total execution time: approximately 2-3 minutes
- All data is saved in JSON format compatible with the SeedData loader
