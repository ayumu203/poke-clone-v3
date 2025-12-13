export interface StarterOption {
    pokemonSpeciesId: number;
    name: string;
    frontImage: string;
    backImage: string;
    type1: number | string;
    type2?: number | string | null;
    baseHp: number;
    baseAttack: number;
    baseDefence: number;
    baseSpecialAttack: number;
    baseSpecialDefence: number;
    baseSpeed: number;
}
