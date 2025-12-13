import * as fs from 'fs';
import * as path from 'path';

interface FlavorTextEntry {
    flavor_text: string;
    language: {
        name: string;
        url: string;
    };
    version: {
        name: string;
        url: string;
    };
}

interface PokemonSpeciesResponse {
    id: number;
    name: string;
    flavor_text_entries: FlavorTextEntry[];
}

interface PokemonDescriptions {
    [key: string]: string;
}

// スターターポケモンのID（第1世代～第9世代）
const STARTER_POKEMON_IDS = [
    1, 4, 7,       // 第1世代: フシギダネ、ヒトカゲ、ゼニガメ
    152, 155, 158, // 第2世代: チコリータ、ヒノアラシ、ワニノコ
    252, 255, 258, // 第3世代: キモリ、アチャモ、ミズゴロウ
    387, 390, 393, // 第4世代: ナエトル、ヒコザル、ポッチャマ
    495, 498, 501, // 第5世代: ツタージャ、ポカブ、ミジュマル
    650, 653, 656, // 第6世代: ハリマロン、フォッコ、ケロマツ
    722, 725, 728, // 第7世代: モクロー、ニャビー、アシマリ
    810, 813, 816, // 第8世代: サルノリ、ヒバニー、メッソン
    906, 909, 912, // 第9世代: ニャオハ、ホゲータ、クワッス
];

async function fetchPokemonDescription(pokemonId: number): Promise<string> {
    try {
        const response = await fetch(`https://pokeapi.co/api/v2/pokemon-species/${pokemonId}`);

        if (!response.ok) {
            throw new Error(`Failed to fetch Pokemon ${pokemonId}: ${response.statusText}`);
        }

        const data: PokemonSpeciesResponse = await response.json();

        // 日本語の説明文を取得（最初に見つかったものを使用）
        const japaneseEntry = data.flavor_text_entries.find(
            (entry) => entry.language.name === 'ja' || entry.language.name === 'ja-Hrkt'
        );

        if (japaneseEntry) {
            // 改行やフォームフィード文字を削除し、スペースに置換
            return japaneseEntry.flavor_text.replace(/[\n\f]/g, ' ').trim();
        }

        // 日本語が見つからない場合は英語を取得
        const englishEntry = data.flavor_text_entries.find(
            (entry) => entry.language.name === 'en'
        );

        if (englishEntry) {
            return englishEntry.flavor_text.replace(/[\n\f]/g, ' ').trim();
        }

        return '説明文が見つかりませんでした。';
    } catch (error) {
        console.error(`Error fetching Pokemon ${pokemonId}:`, error);
        return `エラー: データを取得できませんでした。`;
    }
}

async function main() {
    console.log('PokeAPIからポケモンの説明文を取得中...');

    const descriptions: PokemonDescriptions = {};

    for (const id of STARTER_POKEMON_IDS) {
        console.log(`Pokemon ID ${id} を取得中...`);
        const description = await fetchPokemonDescription(id);
        descriptions[id.toString()] = description;

        // APIのレート制限を考慮して少し待機
        await new Promise(resolve => setTimeout(resolve, 100));
    }

    // JSONファイルとして保存
    const outputDir = path.join(__dirname, '..', 'src', 'data', 'pokemon');
    const outputPath = path.join(outputDir, 'pokemonDescriptions.json');

    // ディレクトリが存在しない場合は作成
    if (!fs.existsSync(outputDir)) {
        fs.mkdirSync(outputDir, { recursive: true });
    }

    fs.writeFileSync(outputPath, JSON.stringify(descriptions, null, 2), 'utf-8');

    console.log(`\n✅ 完了! ${Object.keys(descriptions).length}匹のポケモンデータを保存しました。`);
    console.log(`📁 保存先: ${outputPath}`);
}

main().catch(console.error);
