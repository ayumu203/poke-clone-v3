🤖 コーディングエージェントへの作業指示書
このドキュメントは、プロジェクト「Poke-clone-v3」の実装を担当するエージェントに向けた詳細な作業手順書です。Docs/gemini.md で定義されたコンテキストに基づき、以下のフェーズ順に実装を進めてください。

⚠️ 重要: 全フェーズ共通ルール
SSOTの遵守: すべての実装は Docs/ 以下の設計図および Docs/gemini.md に従うこと。独自の判断で設計を変更しないこと。

コード規約:

コード内にコメントは記述しない（可読性の高いコードで表現する）。

省略せず、ファイル全体を出力する。

非同期処理 (async/await) を基本とする。

エラーハンドリング: 常に例外処理を考慮し、クラッシュしない堅牢なコードを書くこと。

📅 Phase 1: プロジェクト初期化と環境構築
目的: .NETソリューションを作成し、Dockerコンテナとの接続を確立する。

手順
ソリューション作成:

ルートディレクトリに Server ディレクトリを作成（既存の Server/Dockerfile は維持）。

Server 内に PokeClone.sln を作成。

以下のプロジェクトを作成し、ソリューションに追加する（Clean Architecture構成）。

src/Server.Domain (Class Library)

src/Server.Application (Class Library)

src/Server.Infrastructure (Class Library)

src/Server.WebAPI (ASP.NET Core Web API) - ※ここをエントリーポイントとする

tests/Server.Tests (xUnit)

プロジェクト参照の追加:

WebAPI -> Infrastructure, Application, Domain

Infrastructure -> Application, Domain

Application -> Domain

Docker環境の整備:

docker-compose.yml を編集し、app サービスのビルドコンテキストを修正。

Redisサービスを追加（ポート6379）。

WebAPI/appsettings.json にDB接続文字列とRedis接続設定を追加。

完了条件:

dotnet build が成功する。

docker-compose up でDB, Redis, APIが起動し、相互に接続できる。

📅 Phase 2: ドメイン層 (Core Logic) の実装
目的: 外部依存のないビジネスロジックを実装する。

参照資料:

Docs/UML/クラス図.drawio

Docs/UML/ER図.drawio

手順
Entities & Value Objects:

Server.Domain/Entities/ に Pokemon.cs, Move.cs, BattlePlayer.cs, Battle.cs を実装。

Server.Domain/Enums/ に Type.cs, Ailment.cs, Category.cs 等の列挙型を定義。

Server.Domain/Entities/BattleState.cs などのステート管理クラスを実装。

Domain Services (計算ロジック):

Server.Domain/Services/ に以下のクラスとインターフェースを実装。

TypeEffectiveManager (タイプ相性計算)

StatCalculator (ステータス計算)

DamageCalculator (ダメージ計算)

Unit Tests:

Server.Tests に計算ロジックのテストケースを作成（例: ほのおタイプへのみず技の倍率確認）。

完了条件:

ドメインモデルがクラス図通りにコード化されている。

計算ロジックの単体テストが全てパスする。

📅 Phase 3: インフラストラクチャ層の実装
目的: データの永続化と外部サービス連携を実装する。

参照資料:

Docs/UML/ER図.drawio

Docs/要件定義.md (Redisの利用方法)

手順
EF Core Setup:

Server.Infrastructure/Data/AppDbContext.cs を実装。

Fluent API を使用してテーブル定義とリレーションを設定。

Server.Infrastructure/Repositories/SqlRepository.cs (Generic Repository) を実装。

Redis Repository:

Server.Infrastructure/Repositories/RedisBattleRepository.cs を実装。

StackExchange.Redis を使用し、BattleState のJSONシリアライズ保存/取得処理を実装。

排他制御（Locking）のロジックを含める。

Migrations:

dotnet ef migrations add InitialCreate を実行し、DBスキーマを作成。

完了条件:

SQL Serverにテーブルが作成される。

Redisへの読み書きが正常に動作する。

📅 Phase 4: アプリケーション層の実装
目的: ユースケースの制御とリアルタイム通信を実装する。

参照資料:

Docs/UML/シーケンス図.drawio (対戦フロー)

Docs/UML/アクティビティ図.drawio

手順
SignalR Hub:

Server.Application/Hubs/BattleHub.cs を実装。

メソッド: JoinRoom, SubmitAction。

SubmitAction 内でアクションを受信し、全員のアクションが揃ったら BattleService を呼び出す。

Battle Service:

Server.Application/Services/BattleService.cs を実装。

ProcessTurn メソッド: ドメイン層のロジックを呼び出し、ターン処理（先攻/後攻、ダメージ適用、瀕死判定）を一括で行う。

処理結果 (ProcessResult) を作成して返す。

完了条件:

クライアントからのアクション送信に対し、サーバーがターン処理を行い、結果をブロードキャストできる。

📅 Phase 5: WebAPI層とシードデータ
目的: REST APIエンドポイントと初期データの投入を行う。

手順
Controllers:

AuthController: ログイン・JWT発行（モックまたはExternal ID連携）。

PokemonController: ポケモン一覧、詳細データの取得。

UserController: ユーザー情報、手持ちポケモンの取得。

Seed Data:

Server.WebAPI/SeedData.cs を実装。

PokeAPI等のJSONデータ（第1世代）を読み込み、DBに存在しない場合のみInsertする（冪等性を担保）。

Program.cs で起動時に --seed フラグをチェックして実行するロジックを追加。

完了条件:

API経由でポケモンデータを取得できる。

コンテナ起動時にデータが投入される。

📅 Phase 6: フロントエンド実装 (Next.js)
目的: ユーザーインターフェースを実装し、バックエンドと結合する。

参照資料:

Docs/要件定義.md (非機能要件: NES.css)

手順
Setup:

ルートに Client ディレクトリを作成し、Next.js (App Router) プロジェクトを作成。

NES.css をインストールし、layout.tsx に適用。

API Client:

Axios等のインスタンスを作成し、インターセプターでJWTヘッダーを付与する設定を追加。

SignalR Hook:

useSignalR カスタムフックを作成し、接続管理とイベントリスナー登録を共通化。

Pages & Components:

app/battle/[id]/page.tsx: 対戦画面。

components/BattleCanvas.tsx: HPバー、メッセージログ、コマンドボタンの実装。

アニメーション同期ロジックの実装。

完了条件:

ブラウザで対戦画面が表示され、コマンド操作によって対戦が進行する。

🚀 開発開始の合図
まずは Phase 1 から順に作業を開始してください。各フェーズの完了ごとに報告を行い、次の指示を仰いでください。
