export interface PokemonSpecies {
    pokemonSpeciesId: number;
    name: string;
    frontImage: string;
    backImage: string;
    type1: number;
    type2?: number | null;
    baseHp: number;
    baseAttack: number;
    baseDefence: number;
    baseSpecialAttack: number;
    baseSpecialDefence: number;
    baseSpeed: number;
}
