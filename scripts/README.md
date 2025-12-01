# Scripts

このディレクトリには、開発やテストを効率化するためのスクリプトが含まれています。

## setup-battle.sh

PoC用のバトルセットアップを自動化するスクリプトです。

### 前提条件

- Docker Composeでサーバーが起動していること
- `jq`コマンドがインストールされていること
  ```bash
  # Ubuntu/Debian
  sudo apt-get install jq
  
  # macOS
  brew install jq
  ```

### 使用方法

```bash
cd scripts
chmod +x setup-battle.sh
./setup-battle.sh
```

### 出力

スクリプトは以下の情報を出力します:

- **API URL**: `http://localhost:5000`
- **Battle ID**: 作成されたバトルのID（例: `battle-xxx-xxx`）
- **Player ID**: `testplayer1`

これらの情報をPoCクライアント（`Client-PoC/index.html`）で使用してバトルに接続できます。

### トラブルシューティング

#### jqコマンドが見つからない

```bash
# jqをインストール
sudo apt-get install jq  # Ubuntu/Debian
brew install jq          # macOS
```

#### サーバーに接続できない

```bash
# サーバーが起動しているか確認
docker compose ps

# サーバーのログを確認
docker compose logs app
```

#### データベースに接続できない

```bash
# データベースコンテナが起動しているか確認
docker compose ps pokeclone_db

# データベースのログを確認
docker compose logs pokeclone_db
```
