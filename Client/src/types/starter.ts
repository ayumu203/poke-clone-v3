export interface StarterOption {
    pokemonSpeciesId: number;
    name: string;
    frontImage: string;
    backImage: string;
    type1: string;
    type2?: string;
    baseHp: number;
    baseAttack: number;
    baseDefence: number;
    baseSpecialAttack: number;
    baseSpecialDefence: number;
    baseSpeed: number;
}
