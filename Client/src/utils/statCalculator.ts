/**
 * ステータス計算ユーティリティ
 * サーバー側と同じ計算ロジックをフロントエンドにも実装
 */

/**
 * HP実数値を計算
 * 第1世代の計算式: HP = ((Base + IV) * 2 + EVボーナス) * Level / 100 + Level + 10
 * IV=15（最大値）, EV=65535（最大値）を仮定
 * 
 * @param level ポケモンのレベル
 * @param baseHp 種族値のHP
 * @returns HP実数値
 */
export function calcHp(level: number, baseHp: number): number {
    const iv = 15;
    const ev = 65535;
    const evBonus = Math.floor(Math.ceil(Math.sqrt(ev)) / 4);
    return Math.floor(((baseHp + iv) * 2 + evBonus) * level / 100 + level + 10);
}

/**
 * HP以外のステータス実数値を計算
 * 第1世代の計算式: Stat = ((Base + IV) * 2 + EVボーナス) * Level / 100 + 5
 * IV=15（最大値）, EV=65535（最大値）を仮定
 * 
 * @param level ポケモンのレベル
 * @param baseStat 種族値（こうげき、ぼうぎょ、とくこう、とくぼう、すばやさ）
 * @returns ステータス実数値
 */
export function calcStat(level: number, baseStat: number): number {
    const iv = 15;
    const ev = 65535;
    const evBonus = Math.floor(Math.ceil(Math.sqrt(ev)) / 4);
    return Math.floor(((baseStat + iv) * 2 + evBonus) * level / 100 + 5);
}
