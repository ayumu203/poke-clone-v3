# Cluade

# 重要
- 応答は日本語でお願いします.
- 不明点は毎度質問をお願いします.
- 作業内容は適宜Gitにて管理をお願いします(ブランチは現在のまま, コミットを行っていただければ大丈夫です).
- ドキュメントをよく確認してください.

## プロジェクト管理者によるレビュー

- 以下のレビューに対する解答を`レビュー回答.md`として作成してください.

### Server.Domain

- `PlayerParty.cs`にてPlayerPartyIdが定義されていますが, これはPlayerはPlayerPartyを複数持つことはないから, PlayerIdをプライマリキーとすればよいのでは?
    - なにか理由がある場合は説明をいただきたいです.
    - 修正をする場合は報告書へ記載をお願いします.
- `PokemonStat.cs`において, Hpが定義されていますが, こちらはランク補正ではないことから定義の必要性は疑問です.
    - なにか理由がある場合は説明をいただきたいです.
    - 修正をする場合は報告書へ記載をお願いします.
- `Battle.cs`の191~240行で暫定的に実装となっている項目の実装方針の記述をお願いします.
- `Server/scripts/Program.cs`で, `var outputDir = Path.Combine("..", "..", "Server", "seeds");`これパスあってる？
    - moves.jsonの項目数が, Moveのクラスが求めるものと異なっています.
    - 別ディレクトリに正しい, データが存在するのですか？
    - JSONにailment, statChance等の値が正しく取得されるようJSONパースプログラムを確認してください.

### フロントエンド実装手順

- 前回までの変更により, フロントエンドでも変更しなければならない事柄が発生していると思われます.
- フロントエンド実装手順.mdの修正をお願いします.

## ドキュメント

- 特に以下のドキュメントをよく確認して作業をしてください.
  - `/mnt/c/Users/cs23017/Shizuoka University/ドキュメント/dev/01_poke_clone-v3/Docs/Claude.md`
  - `/mnt/c/Users/cs23017/Shizuoka University/ドキュメント/dev/01_poke_clone-v3/Docs/UML/クラス図.drawio`
  - `/mnt/c/Users/cs23017/Shizuoka University/ドキュメント/dev/01_poke_clone-v3/Docs/UML/シーケンス図.drawio`
  - `/mnt/c/Users/cs23017/Shizuoka University/ドキュメント/dev/01_poke_clone-v3/Docs/UML/アーキテクチャ構成.drawio`
  - `/mnt/c/Users/cs23017/Shizuoka University/ドキュメント/dev/01_poke_clone-v3/Docs/フロントエンド実装手順.md`
  - `/mnt/c/Users/cs23017/Shizuoka University/ドキュメント/dev/01_poke_clone-v3/Docs/要件定義.md` 