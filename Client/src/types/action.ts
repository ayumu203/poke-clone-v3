export interface PlayerAction {
    playerId: string;
    actionType: ActionType;
    value: number;  // moveId or switchPokemonIndex
}

export enum ActionType {
    Attack = 0,
    Switch = 1,
    Catch = 2
}
