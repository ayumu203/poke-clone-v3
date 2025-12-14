import { type ActionType, type PlayerAction } from "./action";

export interface BattleState {
  battleId: string;
  player1: PlayerState;
  player2: PlayerState;
  turn: number;
  createdAt: string;
  expireAt?: string;
}

export interface PlayerState {
  playerId: string;
  player: Player;
  activePokemonIndex: number;
  party: PokemonState[];
  action?: PlayerAction;
}

export interface PokemonState {
  pokemonId: string;
  pokemonSpeciesId: number;
  currentHp: number;
  maxHp: number;
  ailment: Ailment;
  rank: Rank;
  isFainted?: boolean;
}

export interface Player {
  playerId: string;
  name: string;
  iconUrl?: string;
}

export interface Rank {
  attack: number;
  defence: number;
  specialAttack: number;
  specialDefence: number;
  specialDefense?: number; // Added to match PokemonStat usually, but defined as specialDefence in doc
  speed: number;
  accuracy: number;
  evasion: number;
}

export type Ailment =
  | "None"
  | "Paralysis"
  | "Sleep"
  | "Freeze"
  | "Burn"
  | "Poison"
  | "DeadlyPoisoned";

export type PokemonStat =
  | "attack"
  | "defense"
  | "special-attack"
  | "special-defense"
  | "speed"
  | "accuracy"
  | "evasion";

export interface StatChange {
  stat: PokemonStat;
  change: number; // -6 ~ +6 の範囲
}

/**
 * バトルの待機状態を表す型
 * - idle: 待機なし（通常状態）
 * - waiting-action: 行動入力待ち
 * - waiting-opponent: 対戦相手の行動待ち
 * - processing: ターン処理中
 */
export type BattleWaitStatus = "idle" | "waiting-action" | "waiting-opponent" | "processing";

export interface ProcessResult {
  actionResults: ActionResult[];
  isBattleEnd: boolean;
  winnerId: string;
  endResult?: EndResult;
  ailmentDamages?: AilmentDamage[];
  hpChanges?: HpChange[];
  forcedSwitches?: ForcedSwitch[];
}

export interface AilmentDamage {
  pokemonId: string;
  ailment: Ailment | number;
  damage: number;
  hpBefore: number;
  hpAfter: number;
}

export interface HpChange {
  pokemonId: string;
  hpBefore: number;
  hpAfter: number;
  maxHp: number;
}

export interface ForcedSwitch {
  playerId: string;
  faintedPokemonId: string;
  nextPokemonId: string;
}

export interface ActionResult {
  actionPokemonId: string;
  actionType: ActionType;
  moveResult?: MoveResult;
  switchResult?: SwitchResult;
  catchResult?: CatchResult;
  escapeResult?: EscapeResult;
}

export interface MoveResult {
  moveId: number;
  targetId: string;
  isSuccess: boolean;
  failureReason: string;
  damage: number;
  hitContext?: {
    isCritical: boolean;
    typeEffectiveness: number;
  };
  sourceRankChange?: Rank;
  targetRankChange?: Rank;
  ailment?: string;
  sourceStatChanges?: StatChange[];
  targetStatChanges?: StatChange[];
  healing?: number;
  drain?: number;
}

export interface SwitchResult {
  currentPokemonId: string;
  nextPokemonId: string;
}

export interface CatchResult {
  isSuccess: boolean;
  caughtPokemonId: string;
}

export interface EscapeResult {
  isSuccess: boolean;
}

export interface EndResult {
  reason: string;
  winnerId?: string;
  evolutionInfo?: EvolutionInfo[];
}

export interface EvolutionInfo {
  pokemonId: string;
  fromSpeciesId: number;
  toSpeciesId: number;
  fromSpeciesName: string;
  toSpeciesName: string;
}
