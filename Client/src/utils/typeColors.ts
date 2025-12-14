/**
 * ポケモンタイプに対応する色のマッピング
 */

export interface TypeColors {
  border: string;
  background: string;
  text: string;
}

export const TYPE_COLORS: Record<string, TypeColors> = {
  // ノーマル
  ノーマル: {
    border: "#A8A878",
    background: "#F5F5DC",
    text: "#5A5A5A",
  },
  normal: {
    border: "#A8A878",
    background: "#F5F5DC",
    text: "#5A5A5A",
  },

  // ほのお
  ほのお: {
    border: "#F08030",
    background: "#FFE4CC",
    text: "#C84000",
  },
  fire: {
    border: "#F08030",
    background: "#FFE4CC",
    text: "#C84000",
  },

  // みず
  みず: {
    border: "#6890F0",
    background: "#E0F0FF",
    text: "#2850C8",
  },
  water: {
    border: "#6890F0",
    background: "#E0F0FF",
    text: "#2850C8",
  },

  // でんき
  でんき: {
    border: "#F8D030",
    background: "#FFF8CC",
    text: "#D0A000",
  },
  electric: {
    border: "#F8D030",
    background: "#FFF8CC",
    text: "#D0A000",
  },

  // くさ
  くさ: {
    border: "#78C850",
    background: "#E8F8E0",
    text: "#408020",
  },
  grass: {
    border: "#78C850",
    background: "#E8F8E0",
    text: "#408020",
  },

  // こおり
  こおり: {
    border: "#98D8D8",
    background: "#E0F8F8",
    text: "#4090A0",
  },
  ice: {
    border: "#98D8D8",
    background: "#E0F8F8",
    text: "#4090A0",
  },

  // かくとう
  かくとう: {
    border: "#C03028",
    background: "#FFE0E0",
    text: "#A02020",
  },
  fighting: {
    border: "#C03028",
    background: "#FFE0E0",
    text: "#A02020",
  },

  // どく
  どく: {
    border: "#A040A0",
    background: "#F0E0F0",
    text: "#702070",
  },
  poison: {
    border: "#A040A0",
    background: "#F0E0F0",
    text: "#702070",
  },

  // じめん
  じめん: {
    border: "#E0C068",
    background: "#F8F0D8",
    text: "#A08040",
  },
  ground: {
    border: "#E0C068",
    background: "#F8F0D8",
    text: "#A08040",
  },

  // ひこう
  ひこう: {
    border: "#A890F0",
    background: "#E8E0FF",
    text: "#6858C0",
  },
  flying: {
    border: "#A890F0",
    background: "#E8E0FF",
    text: "#6858C0",
  },

  // エスパー
  エスパー: {
    border: "#F85888",
    background: "#FFE0F0",
    text: "#C02060",
  },
  psychic: {
    border: "#F85888",
    background: "#FFE0F0",
    text: "#C02060",
  },

  // むし
  むし: {
    border: "#A8B820",
    background: "#F0F8CC",
    text: "#708018",
  },
  bug: {
    border: "#A8B820",
    background: "#F0F8CC",
    text: "#708018",
  },

  // いわ
  いわ: {
    border: "#B8A038",
    background: "#F8F0D0",
    text: "#886830",
  },
  rock: {
    border: "#B8A038",
    background: "#F8F0D0",
    text: "#886830",
  },

  // ゴースト
  ゴースト: {
    border: "#705898",
    background: "#E8E0F0",
    text: "#483060",
  },
  ghost: {
    border: "#705898",
    background: "#E8E0F0",
    text: "#483060",
  },

  // ドラゴン
  ドラゴン: {
    border: "#7038F8",
    background: "#E8DCFF",
    text: "#4818C0",
  },
  dragon: {
    border: "#7038F8",
    background: "#E8DCFF",
    text: "#4818C0",
  },

  // あく
  あく: {
    border: "#705848",
    background: "#E8E0D8",
    text: "#483830",
  },
  dark: {
    border: "#705848",
    background: "#E8E0D8",
    text: "#483830",
  },

  // はがね
  はがね: {
    border: "#B8B8D0",
    background: "#F0F0F8",
    text: "#787898",
  },
  steel: {
    border: "#B8B8D0",
    background: "#F0F0F8",
    text: "#787898",
  },

  // フェアリー
  フェアリー: {
    border: "#EE99AC",
    background: "#FFE8F0",
    text: "#D05078",
  },
  fairy: {
    border: "#EE99AC",
    background: "#FFE8F0",
    text: "#D05078",
  },
};

/**
 * タイプ名に対応する色を取得
 * @param type タイプ名（日本語または英語）
 * @returns タイプに対応する色、見つからない場合はデフォルト色
 */
export function getTypeColors(type: string | undefined | null): TypeColors {
  if (!type || typeof type !== "string") {
    return TYPE_COLORS["ノーマル"];
  }
  const normalizedType = type.trim();
  return TYPE_COLORS[normalizedType] || TYPE_COLORS["ノーマル"];
}
