# Cluade

# 重要
- 応答は日本語でお願いします.
- 不明点は毎度質問をお願いします.
- 作業内容は適宜Gitにて管理をお願いします(ブランチは現在のまま, コミットを行っていただければ大丈夫です).
- ドキュメントをよく確認してください.

## 作業内容

- test-battle.shとsetup-battle.shどっちかでよくない?
- まだMove not found
- デザインをもっと下のHTMLのCSSによせてほしい.
  - 色, 3画面分割(プレイヤー, 対戦情報, 相手プレイヤー), 

```json
{
    "actionResults": [
        {
            "actionPokemonId": "55bece0b-20ec-4618-8497-365da5b28bec",
            "actionType": 0,
            "moveResult": {
                "moveId": 3,
                "targetId": "1778c9de-dab4-42d5-a2a8-d6a49a76ab9a",
                "isSuccess": false,
                "failureReason": "Move not found",
                "damage": 0,
                "hitContext": null,
                "sourceStatChanges": [],
                "targetStatChanges": [],
                "ailment": null,
                "healing": 0,
                "drain": 0
            },
            "switchResult": null,
            "catchResult": null,
            "escapeResult": null
        },
        {
            "actionPokemonId": "1778c9de-dab4-42d5-a2a8-d6a49a76ab9a",
            "actionType": 0,
            "moveResult": {
                "moveId": 14,
                "targetId": "55bece0b-20ec-4618-8497-365da5b28bec",
                "isSuccess": true,
                "failureReason": "",
                "damage": 0,
                "hitContext": {
                    "isCritical": false,
                    "typeEffectiveness": 1
                },
                "sourceStatChanges": [],
                "targetStatChanges": [],
                "ailment": null,
                "healing": 0,
                "drain": 0
            },
            "switchResult": null,
            "catchResult": null,
            "escapeResult": null
        }
    ],
    "isBattleEnd": false,
    "winnerId": "",
    "endResult": null
}
```


```html
<!DOCTYPE html>
<html lang="jp">
<head>
<style type="text/css">
    table {
        border-collapse: separate;
        border-spacing: 0px;
    }
    tr,td {
        padding: 0px;
    }
    td.reg {
        font-size: large;
    }
    td.R6radio {
        width: 45px;
        text-align: right;
        padding: 0px 5px 0px 0px;
    }
    input.variable {
        width: 70px;
        font-size: large;
    }
    input.variable2 {
        width: 120px;
        font-size: large;
    }
    input.variable3 {
        width: 180px;
        font-size: large;
    }
    span.address {
	padding: 0pt 8pt;
        font-size: large;
    }
    label.address {
	padding: 0pt 8pt;
        font-size: large;
    }
    select.variable2 {
        width: 120px;
        font-size: large;
    }
    select.variable3 {
        width: 180px;
        font-size: large;
    }
    button.run {
        font-size: large;
    }
    .flex{
        display: flex;
        border: 1px solid #333;
    }
    .flex div{
        border: 1px solid #333;
    }
    .middle{
        width: 700px;
        background: #ffd87c;
    }
    .left{
        width: 500px;
        background: #9ddbfc;
    }
    .right{
	width: 800px;
        background: #9dfcdb;
    }
    .freeArea{
        max-width: 100%;
        width: 900%;
        height: 800px;
        font-size: 18pt;
    }
    span.stackValue {
        width: 200px;
        background-color: white;
        border-style: solid;
        border-width: 1px 1px 1px 1px;
        border-color: blue;
    }
</style>
```

## ドキュメント

- 特に以下のドキュメントをよく確認して作業をしてください.
  - `/mnt/c/Users/cs23017/Shizuoka University/ドキュメント/dev/01_poke_clone-v3/Docs/Claude.md`
  - `/mnt/c/Users/cs23017/Shizuoka University/ドキュメント/dev/01_poke_clone-v3/Docs/UML/クラス図.drawio`
  - `/mnt/c/Users/cs23017/Shizuoka University/ドキュメント/dev/01_poke_clone-v3/Docs/UML/シーケンス図.drawio`
  - `/mnt/c/Users/cs23017/Shizuoka University/ドキュメント/dev/01_poke_clone-v3/Docs/UML/アーキテクチャ構成.drawio`
  - `/mnt/c/Users/cs23017/Shizuoka University/ドキュメント/dev/01_poke_clone-v3/Docs/フロントエンド実装手順.md`
  - `/mnt/c/Users/cs23017/Shizuoka University/ドキュメント/dev/01_poke_clone-v3/Docs/要件定義.md` 