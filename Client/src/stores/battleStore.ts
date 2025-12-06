import { create } from 'zustand';

import { type BattleState, type PokemonState,type ProcessResult } from '@/types/battle';

interface BattleStore {
    battleState: BattleState | null;
    turnResult: ProcessResult | null;
    battleLog: string[];
    isWaitingForOpponent: boolean;

    setBattleState: (state: BattleState) => void;
    setTurnResult: (result: ProcessResult) => void;
    addLog: (message: string) => void;
    clearLog: () => void;
    updatePokemonHp: (pokemonId: string, damage: number) => void;
    setWaitingForOpponent: (waiting: boolean) => void;
    resetBattle: () => void;
}

export const useBattleStore = create<BattleStore>((set) => ({
    battleState: null,
    turnResult: null,
    battleLog: [],
    isWaitingForOpponent: false,

    setBattleState: (state) => set({ battleState: state }),

    setTurnResult: (result) => set({ turnResult: result }),

    addLog: (message) => set((state) => ({
        battleLog: [...state.battleLog, message]
    })),

    clearLog: () => set({ battleLog: [] }),

    updatePokemonHp: (pokemonId, damage) => set((state) => {
        if (!state.battleState) return state;

        const updateParty = (party: PokemonState[]) =>
            party.map(p =>
                p.pokemonId === pokemonId
                    ? { ...p, currentHp: Math.max(0, p.currentHp - damage) }
                    : p
            );

        return {
            battleState: {
                ...state.battleState,
                player1: {
                    ...state.battleState.player1,
                    party: updateParty(state.battleState.player1.party)
                },
                player2: {
                    ...state.battleState.player2,
                    party: updateParty(state.battleState.player2.party)
                }
            }
        };
    }),

    setWaitingForOpponent: (waiting) => set({ isWaitingForOpponent: waiting }),

    resetBattle: () => set({
        battleState: null,
        turnResult: null,
        battleLog: [],
        isWaitingForOpponent: false
    })
}));
