"use client";

import type { BattleState, ProcessResult } from "@/types/battle";
import type { Pokemon } from "@/types/pokemon";
import type { PokemonSpecies } from "@/types/pokemonSpecies";

// 状態異常のEnum値から日本語名へのマップ
const AILMENT_LABELS: Record<number | string, string> = {
    0: "",
    1: "まひ",
    2: "ねむり",
    3: "こおり",
    4: "やけど",
    5: "どく",
    6: "もうどく",
    // 文字列キー版
    "None": "",
    "Paralysis": "まひ",
    "Sleep": "ねむり",
    "Freeze": "こおり",
    "Burn": "やけど",
    "Poison": "どく",
    "DeadlyPoisoned": "もうどく",
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
            const moveResult = actionResult.moveResult;
            const moveName = getMoveName(moveResult.moveId, party, battleState);
            const targetName = getPokemonName(moveResult.targetId, battleState, species);

            if (!moveResult.isSuccess) {
                logs.push(`${attackerName}の${moveName}は失敗した！`);
                continue;
            }

            // ダメージ技
            if (moveResult.damage > 0) {
                logs.push(`${attackerName}は${targetName}に${moveName}を使い、${moveResult.damage}ダメージを与えた！`);

                if (moveResult.hitContext?.isCritical) {
                    logs.push("急所に当たった！");
                }

                if (moveResult.hitContext?.typeEffectiveness === 2) {
                    logs.push("効果は抜群だ！");
                } else if (moveResult.hitContext?.typeEffectiveness === 0.5) {
                    logs.push("効果はいまひとつ...");
                } else if (moveResult.hitContext?.typeEffectiveness === 0) {
                    logs.push("効果がないようだ...");
                }
            } else {
                // 変化技（ダメージなし）
                logs.push(`${attackerName}は${moveName}を使った！`);
            }

            // ステータス変化（攻撃者側）
            if (moveResult.sourceStatChanges && moveResult.sourceStatChanges.length > 0) {
                for (const statChange of moveResult.sourceStatChanges) {
                    const statName = STAT_LABELS[statChange.stat as unknown as number] || "ステータス";
                    const direction = statChange.change > 0 ? "上がった" : "下がった";
                    const amount = Math.abs(statChange.change) > 1 ? "ぐーんと" : "";
                    logs.push(`${attackerName}の${statName}が${amount}${direction}！`);
                }
            }

            // ステータス変化（ターゲット側）
            if (moveResult.targetStatChanges && moveResult.targetStatChanges.length > 0) {
                for (const statChange of moveResult.targetStatChanges) {
                    const statName = STAT_LABELS[statChange.stat as unknown as number] || "ステータス";
                    const direction = statChange.change > 0 ? "上がった" : "下がった";
                    const amount = Math.abs(statChange.change) > 1 ? "ぐーんと" : "";
                    logs.push(`${targetName}の${statName}が${amount}${direction}！`);
                }
            }

            // 状態異常付与
            if (moveResult.ailment != null) {
                const ailmentKey = moveResult.ailment;
                const ailmentName = AILMENT_LABELS[ailmentKey as keyof typeof AILMENT_LABELS];

                if (ailmentName && ailmentName !== "") {
                    logs.push(`${targetName}は${ailmentName}状態になった！`);
                }
            }

            // 回復
            if (moveResult.healing && moveResult.healing > 0) {
                logs.push(`${attackerName}は${moveResult.healing}回復した！`);
            }

            // 吸収
            if (moveResult.drain && moveResult.drain > 0) {
                logs.push(`${attackerName}は${moveResult.drain}吸い取った！`);
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

    // 状態異常ダメージ
    if (result.ailmentDamages && result.ailmentDamages.length > 0) {
        for (const ailmentDmg of result.ailmentDamages) {
            const pokemonName = getPokemonName(ailmentDmg.pokemonId, battleState, species);
            const ailmentName = AILMENT_LABELS[ailmentDmg.ailment as keyof typeof AILMENT_LABELS] || "状態異常";
            logs.push(`${pokemonName}は${ailmentName}で${ailmentDmg.damage}ダメージ！ (HP: ${ailmentDmg.hpBefore}→${ailmentDmg.hpAfter})`);
        }
    }

    // 強制交代（ポケモンが瀕死になった時）
    if (result.forcedSwitches && result.forcedSwitches.length > 0) {
        for (const forcedSwitch of result.forcedSwitches) {
            const faintedName = getPokemonName(forcedSwitch.faintedPokemonId, battleState, species);
            const nextName = getPokemonName(forcedSwitch.nextPokemonId, battleState, species);
            logs.push(`${faintedName}は倒れた！`);
            logs.push(`${nextName}を繰り出した！`);
        }
    }

    // HP変化の表示
    if (result.hpChanges && result.hpChanges.length > 0) {
        for (const hpInfo of result.hpChanges) {
            const pokemonName = getPokemonName(hpInfo.pokemonId, battleState, species);
            logs.push(`[HP] ${pokemonName}: ${hpInfo.hpAfter}/${hpInfo.maxHp}`);
        }
    }

    // バトル終了判定
    if (result.isBattleEnd && result.winnerId) {
        logs.push("バトル終了！");
    }

    return logs;
}
