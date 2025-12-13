/**
 * ポケモンタイプのEnum定義と変換ユーティリティ
 */

/**
 * ポケモンタイプのEnum値
 * APIから返される数値とタイプ名のマッピング
 */
export enum PokemonTypeEnum {
    ノーマル = 0,
    ほのお = 1,
    みず = 2,
    でんき = 3,
    くさ = 4,
    こおり = 5,
    かくとう = 6,
    どく = 7,
    じめん = 8,
    ひこう = 9,
    エスパー = 10,
    むし = 11,
    いわ = 12,
    ゴースト = 13,
    ドラゴン = 14,
    あく = 15,
    はがね = 16,
    フェアリー = 17,
}

/**
 * Enum値から日本語タイプ名への変換マップ
 */
const TYPE_ENUM_TO_NAME: Record<number, string> = {
    0: 'ノーマル',
    1: 'ほのお',
    2: 'みず',
    3: 'でんき',
    4: 'くさ',
    5: 'こおり',
    6: 'かくとう',
    7: 'どく',
    8: 'じめん',
    9: 'ひこう',
    10: 'エスパー',
    11: 'むし',
    12: 'いわ',
    13: 'ゴースト',
    14: 'ドラゴン',
    15: 'あく',
    16: 'はがね',
    17: 'フェアリー',
};

/**
 * タイプのEnum値を文字列に変換
 * @param typeEnum タイプのEnum値（数値または文字列）
 * @returns タイプ名（日本語）、不明な場合は'ノーマル'
 */
export function convertTypeEnumToString(typeEnum: number | string | undefined | null): string {
    // すでに文字列の場合はそのまま返す
    if (typeof typeEnum === 'string') {
        return typeEnum;
    }

    // 数値の場合は変換
    if (typeof typeEnum === 'number') {
        return TYPE_ENUM_TO_NAME[typeEnum] || 'ノーマル';
    }

    // それ以外はデフォルト
    return 'ノーマル';
}
