export interface Pokemon {
    pokemonId: string;
    pokemonSpeciesId: number;
    level: number;
    exp: number;
    moves: Move[];
}

export interface Move {
    moveId: number;
    name: string;
    type: string;
    damageClass: string;
    power: number;
    accuracy: number;
    pp: number;
}
