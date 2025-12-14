"use client";

import type { BattleState, ProcessResult } from "@/types/battle";
import type { Pokemon } from "@/types/pokemon";
import type { PokemonSpecies } from "@/types/pokemonSpecies";

// 状態異常のEnum値から日本語名へのマップ
const AILMENT_LABELS: Record<number, string> = {
    0: "",
    1: "まひ",
    2: "ねむり",
    3: "こおり",
    4: "やけど",
    5: "どく",
    6: "もうどく",
};

// ステータスのEnum値から日本語名へのマップ
const STAT_LABELS: Record<number, string> = {
    0: "こうげき",
    1: "ぼうぎょ",
    2: "とくこう",
    3: "とくぼう",
    4: "すばやさ",
    5: "めいちゅう",
    6: "かいひ",
};

interface LogContext {
    battleState: BattleState;
    species: (PokemonSpecies | undefined)[];
    party: Pokemon[];
}

/**
 * ポケモンIDから名前を取得
 */
function getPokemonName(
    pokemonId: string,
    battleState: BattleState,
    species: (PokemonSpecies | undefined)[]
): string {
    const allParty = [
        ...battleState.player1.party,
        ...battleState.player2.party,
    ];
    const pokemonIndex = allParty.findIndex((p) => p.pokemonId === pokemonId);
    if (pokemonIndex >= 0 && species[pokemonIndex]) {
        return species[pokemonIndex]!.name;
    }
    return "???";
}

/**
 * moveIdから技名を取得
 * partyに加え、battleState内の全ポケモンからも技を検索する
 */
function getMoveName(
    moveId: number,
    party: Pokemon[],
    battleState: BattleState
): string {
    // まずプレイヤーのパーティから検索
    for (const pokemon of party) {
        const move = pokemon.moves?.find((m) => m.moveId === moveId);
        if (move) {
            return move.name;
        }
    }

    // battleState内のPokemonEntityから技名を取得（moveListがある場合）
    const allPokemonEntities = [
        ...(battleState.player1 as unknown as PlayerStateWithEntities)
            .pokemonEntities || [],
        ...(battleState.player2 as unknown as PlayerStateWithEntities)
            .pokemonEntities || [],
    ];

    for (const entity of allPokemonEntities) {
        if (entity?.moves) {
            const move = entity.moves.find(
                (m: { moveId: number; name?: string }) => m.moveId === moveId
            );
            if (move?.name) {
                return move.name;
            }
        }
    }

    return `技${moveId}`;
}

// battleStateの拡張型（サーバーから返されるpokemonEntitiesを含む）
interface PlayerStateWithEntities {
    pokemonEntities?: Array<{
        moves?: Array<{ moveId: number; name?: string }>;
    }>;
}

/**
 * ターン結果からわかりやすいログメッセージを生成
 */
export function generateBattleLogs(
    result: ProcessResult,
    context: LogContext
): string[] {
    const logs: string[] = [];
    const { battleState, species, party } = context;

    for (const actionResult of result.actionResults) {
        const attackerName = getPokemonName(actionResult.actionPokemonId, battleState, species);

        if (actionResult.moveResult) {
            const mr = actionResult.moveResult;
            const moveName = getMoveName(mr.moveId, party, battleState);
            const targetName = getPokemonName(mr.targetId, battleState, species);

            if (!mr.isSuccess) {
                logs.push(`${attackerName}の${moveName}は失敗した！`);
                continue;
            }

            // ダメージ技
            if (mr.damage > 0) {
                logs.push(`${attackerName}は${targetName}に${moveName}を使い、${mr.damage}ダメージを与えた！`);

                if (mr.hitContext?.isCritical) {
                    logs.push("急所に当たった！");
                }

                if (mr.hitContext?.typeEffectiveness === 2) {
                    logs.push("効果は抜群だ！");
                } else if (mr.hitContext?.typeEffectiveness === 0.5) {
                    logs.push("効果はいまひとつ...");
                } else if (mr.hitContext?.typeEffectiveness === 0) {
                    logs.push("効果がないようだ...");
                }
            } else {
                // 変化技（ダメージなし）
                logs.push(`${attackerName}は${moveName}を使った！`);
            }

            // ステータス変化
            if (mr.sourceStatChanges && mr.sourceStatChanges.length > 0) {
                for (const change of mr.sourceStatChanges) {
                    const statName = STAT_LABELS[change.stat as unknown as number] || "ステータス";
                    const direction = change.change > 0 ? "上がった" : "下がった";
                    const amount = Math.abs(change.change) > 1 ? "ぐーんと" : "";
                    logs.push(`${attackerName}の${statName}が${amount}${direction}！`);
                }
            }

            if (mr.targetStatChanges && mr.targetStatChanges.length > 0) {
                for (const change of mr.targetStatChanges) {
                    const statName = STAT_LABELS[change.stat as unknown as number] || "ステータス";
                    const direction = change.change > 0 ? "上がった" : "下がった";
                    const amount = Math.abs(change.change) > 1 ? "ぐーんと" : "";
                    logs.push(`${targetName}の${statName}が${amount}${direction}！`);
                }
            }

            // 状態異常付与（numberまたはstringのどちらでも対応）
            if (mr.ailment != null) {
                const ailmentValue = typeof mr.ailment === "number"
                    ? mr.ailment
                    : parseInt(mr.ailment as string, 10);

                if (!isNaN(ailmentValue) && ailmentValue > 0) {
                    const ailmentName = AILMENT_LABELS[ailmentValue] || "状態異常";
                    logs.push(`${targetName}は${ailmentName}状態になった！`);
                } else if (typeof mr.ailment === "string" && mr.ailment !== "None" && mr.ailment !== "0") {
                    // 文字列の場合（"Poison"など）
                    const ailmentLabelsStr: Record<string, string> = {
                        "Paralysis": "まひ",
                        "Sleep": "ねむり",
                        "Freeze": "こおり",
                        "Burn": "やけど",
                        "Poison": "どく",
                        "DeadlyPoisoned": "もうどく",
                    };
                    const ailmentName = ailmentLabelsStr[mr.ailment] || mr.ailment;
                    logs.push(`${targetName}は${ailmentName}状態になった！`);
                }
            }

            // 回復
            if (mr.healing && mr.healing > 0) {
                logs.push(`${attackerName}は${mr.healing}回復した！`);
            }

            // 吸収
            if (mr.drain && mr.drain > 0) {
                logs.push(`${attackerName}は${mr.drain}吸い取った！`);
            }
        }

        if (actionResult.catchResult) {
            if (actionResult.catchResult.isSuccess) {
                logs.push("やったー！捕まえた！");
            } else {
                logs.push("あっ！捕まえられなかった！");
            }
        }

        if (actionResult.escapeResult) {
            if (actionResult.escapeResult.isSuccess) {
                logs.push("うまく逃げ切れた！");
            } else {
                logs.push("逃げられなかった！");
            }
        }
    }

    // バトル終了判定
    if (result.isBattleEnd && result.winnerId) {
        logs.push("バトル終了！");
    }

    return logs;
}

/**
 * 状態異常ダメージのログを生成（将来の拡張用）
 */
export function generateAilmentDamageLog(
    pokemonName: string,
    ailment: number,
    damage: number
): string {
    const ailmentName = AILMENT_LABELS[ailment] || "状態異常";
    return `${pokemonName}は${ailmentName}で${damage}ダメージを受けた！`;
}
