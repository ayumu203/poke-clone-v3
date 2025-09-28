はい、承知いたしました。対戦ロジックの根幹を担うクラスから定義していくのは、堅牢なシステムを構築する上で非常に良いアプローチです。

添付ファイルのPage-2にあるクラス群を基盤とし、それらを拡張・連携させる形で、**対戦のコアロジック**に必要なクラス・フィールド・メソッドを以下に列挙します。

---

### ## 対戦コアロジックのクラス設計案

対戦ロジックは、大きく分けて**「進行を管理するクラス」「具体的な処理を実行するクラス」「結果を通知するクラス」**の3つに分類して設計すると見通しが良くなります。

### ### 1. 進行管理クラス 🎼

対戦全体の流れやターンの進行をオーケストレーション（指揮）するクラス群です。

#### **`Battle` クラス**
対戦全体の状態を管理する、まさに対戦そのものを表すクラスです。

* **役割**: 2つのパーティ、場に出ているポケモン、天候など、対戦に関する全ての状態を保持し、ターンの実行命令を出します。
* **主要なフィールド**:
    * [cite_start]`player1Party: PlayerParty` [cite: 146, 345]
    * [cite_start]`player2Party: PlayerParty` [cite: 147, 346]
    * [cite_start]`player1ActivePokemon: Pokemon` [cite: 144, 343]
    * [cite_start]`player2ActivePokemon: Pokemon` [cite: 145, 344]
    * [cite_start]`turnNumber: int` [cite: 143, 342]
    * `weather: WeatherState` (天候状態: なし, あめ, はれ など)
    * `field: FieldState` (フィールド状態: エレキフィールドなど)
* **主要なメソッド**:
    * `StartBattle(party1: PlayerParty, party2: PlayerParty)`: 対戦を開始し、最初のポケモンを場に出します。
    * `ExecuteTurn(action1: PlayerAction, action2: PlayerAction): TurnResult`: 2人のプレイヤーのアクションを受け取り、`TurnProcessor` を使ってターンを処理し、その結果を返します。
    * `CheckWinCondition(): Player | null`: どちらかのパーティが全滅したかなど、勝利条件をチェックします。

---

### ### 2. 実行クラス 🏃‍♂️

技の効果やダメージ計算など、具体的な処理を実行する責務を持つクラス群です。

#### **`ActionExecutor` クラス**
プレイヤーの単一のアクション（技の使用、ポケモン交代など）を実際に実行するクラスです。

* **役割**: 「AがBにCの技を使った」という1つのアクションを解釈し、ダメージ計算や状態異常の付与など、関連する処理を各専門クラスに依頼します。
* **主要なメソッド**:
    * `Execute(battle: Battle, action: PlayerAction): List<BattleEvent>`: 1つのアクションを実行し、それによって発生したイベントのリスト（例：「技を使った」「ダメージを与えた」）を返します。

#### **`DamageCalculator` クラス**
[cite_start]既存のクラスを拡張し、より詳細な計算を行います [cite: 170, 355]。

* **役割**: ダメージ計算式に基づいて、純粋なダメージ値を算出することに特化します。
* **主要なフィールド**:
    * `typeEffectivenessManager: TypeEffectivenessManager`
* **主要なメソッド**:
    * `Calculate(attacker: Pokemon, defender: Pokemon, move: Move, battle: Battle): DamageResult`: ダメージ計算に必要な全ての情報を受け取り、計算結果を`DamageResult`オブジェクトとして返します。`battle`オブジェクトを渡すことで、天候などの影響も考慮できるようにします。

#### **`EffectApplier` クラス**
技の追加効果（ステータス変更、状態異常など）を適用するクラスです。

* **役割**: `Move`データが持つ「30%の確率で相手を麻痺させる」といった情報を解釈し、確率計算を行い、実際に相手の`Pokemon`オブジェクトの状態を更新します。
* **主要なメソッド**:
    * `ApplyMoveEffects(attacker: Pokemon, defender: Pokemon, move: Move): List<BattleEvent>`: 技が持つ追加効果（能力ランク変化、状態異常など）を適用し、発生したイベントを返します。

---

### ### 3. 結果通知クラス 📨

処理の結果、何が起きたのかを構造化して格納するためのデータクラス（DTO）群です。

#### **`TurnResult` クラス**
1ターンの実行結果の全てを格納します。

* **役割**: クライアント（フロントエンド）にこのオブジェクトを渡すだけで、1ターン分の出来事を完全に再現できるようにします。
* **主要なフィールド**:
    * `events: List<BattleEvent>`: ターン中に発生した全てのイベントのリスト。
    * `updatedBattleState: BattleStateDto`: ターン終了後の最新の対戦状態のスナップショット。

#### **`BattleEvent` 抽象クラス**
対戦中に発生する全ての「出来事」の基底クラスです。

* **役割**: 対戦ログの1行に相当する情報を構造化します。これを継承して具体的なイベントクラスを定義します。
* **主要なフィールド**:
    * `eventType: string`
    * `messageForLog: string` (例: 「ピカチュウの 10まんボルト！」)
* **具象クラスの例**:
    * `MoveUsedEvent`: `sourcePlayerId`, `targetPokemonId`, `moveName`
    * `DamageDealtEvent`: `targetPokemonId`, `damage`, `isCritical`, `effectiveness`
    * `PokemonFaintedEvent`: `faintedPokemonId`
    * `StatusAppliedEvent`: `targetPokemonId`, `statusType` (e.g., "paralysis")

#### **`DamageResult` クラス**
`DamageCalculator`の計算結果を格納します。

* **役割**: 単純なダメージ数値だけでなく、クリティカルヒットの有無や効果抜群などの情報もまとめて返します。
* **主要なフィールド**:
    * `damage: int`
    * `isCritical: bool`
    * `effectiveness: float` (e.g., 2.0, 0.5, 1.0)
    * `didMiss: bool`

これらのクラスを組み合わせることで、各クラスの責務が明確になり、テストがしやすく拡張性の高い対戦ロジックを構築できます。

