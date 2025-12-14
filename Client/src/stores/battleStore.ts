import { create } from "zustand";

import type { BattleState, BattleWaitStatus, PokemonState, ProcessResult } from "@/types/battle";

interface BattleStore {
  battleState: BattleState | null;
  turnResult: ProcessResult | null;
  battleLog: string[];
  waitStatus: BattleWaitStatus;

  setBattleState: (state: BattleState) => void;
  setTurnResult: (result: ProcessResult) => void;
  addLog: (message: string) => void;
  clearLog: () => void;
  updatePokemonHp: (pokemonId: string, damage: number) => void;
  updatePokemonState: (pokemonId: string, updates: Partial<PokemonState>) => void;
  setWaitStatus: (status: BattleWaitStatus) => void;
  resetBattle: () => void;
}

export const useBattleStore = create<BattleStore>((set) => ({
  battleState: null,
  turnResult: null,
  battleLog: [],
  waitStatus: "idle",

  setBattleState: (state) => set({ battleState: state }),

  setTurnResult: (result) => set({ turnResult: result }),

  addLog: (message) =>
    set((state) => ({
      battleLog: [...state.battleLog, message],
    })),

  clearLog: () => set({ battleLog: [] }),

  updatePokemonHp: (pokemonId, damage) =>
    set((state) => {
      if (!state.battleState) return state;

      const updateParty = (party: PokemonState[]) =>
        party.map((p) =>
          p.pokemonId === pokemonId ? { ...p, currentHp: Math.max(0, p.currentHp - damage) } : p
        );

      return {
        battleState: {
          ...state.battleState,
          player1: {
            ...state.battleState.player1,
            party: updateParty(state.battleState.player1.party),
          },
          player2: {
            ...state.battleState.player2,
            party: updateParty(state.battleState.player2.party),
          },
        },
      };
    }),

  /**
   * ポケモンの状態を部分更新する（HP、状態異常、ランクなど）
   * @param pokemonId 更新対象のポケモンID
   * @param updates 更新する内容（部分的なPokemonState）
   */
  updatePokemonState: (pokemonId, updates) =>
    set((state) => {
      if (!state.battleState) return state;

      const updateParty = (party: PokemonState[]) =>
        party.map((p) => (p.pokemonId === pokemonId ? { ...p, ...updates } : p));

      return {
        battleState: {
          ...state.battleState,
          player1: {
            ...state.battleState.player1,
            party: updateParty(state.battleState.player1.party),
          },
          player2: {
            ...state.battleState.player2,
            party: updateParty(state.battleState.player2.party),
          },
        },
      };
    }),

  setWaitStatus: (status) => set({ waitStatus: status }),

  resetBattle: () =>
    set({
      battleState: null,
      turnResult: null,
      battleLog: [],
      waitStatus: "idle",
    }),
}));
