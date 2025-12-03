# Cluade

# 重要
- 応答は日本語でお願いします.
- 不明点は毎度質問をお願いします.
- 作業内容は適宜Gitにて管理をお願いします(ブランチは現在のまま, コミットを行っていただければ大丈夫です).
- ドキュメントをよく確認してください.

## 作業内容

- 以下JSONのようにMove not foundの原因究明をお願いします.
  MoveIdを間違えて使用していたりする？
  - まだNot found
  - 追加でエラー
- 下記HTMLのようなUIにしてほしい.
  - 機能は今のままで十分なので変更しないでokです.
  - CSSをできるだけ寄せてほしい.
  - 紫の背景とかダサすぎるので.

```bash
$ ./setup-battle.sh
cs23017@DESKTOP-AO278P9:/mnt/c/Users/cs23017/Shizuoka University/ドキュメント/dev/01_poke_clone-v3/scripts$ ./test-battle.sh
=== ポケモンバトルテストスクリプト ===
PlayerId: 1764766996testplayer

1. 認証中...
✅ 認証成功
トークン: eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJodHRwOi8vc...

2. スターターポケモンの選択肢を取得中...
✅ スターターポケモン取得成功
選択肢: ヒコザル(390), ゼニガメ(7), ツタージャ(495)

3. スターターポケモンを選択中 (ヒコザル)...
❌ スターターポケモン選択失敗
レスポンス: System.InvalidOperationException: Player with ID 'c9b29123-079a-4011-826f-028cfc3c8ef4' does not exist. Please create a player profile first.
   at Server.Infrastructure.Repositories.PokemonRepository.AddToPartyAsync(String playerId, Pokemon pokemon) in /src/src/Server.Infrastructure/Repositories/PokemonRepository.cs:line 54
   at Server.WebAPI.Controllers.StarterController.SelectStarter(SelectStarterRequest request) in /src/src/Server.WebAPI/Controllers/StarterController.cs:line 94
   at lambda
```

```json
{
    "actionResults": [
        {
            "actionPokemonId": "0e32b4a6-7b08-4c4c-a1e5-f7756a4dacf1",
            "actionType": 0,
            "moveResult": {
                "moveId": 3,
                "targetId": "cc70ef7d-51ba-4e16-a79a-58d4e8fe1dc3",
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
            "actionPokemonId": "cc70ef7d-51ba-4e16-a79a-58d4e8fe1dc3",
            "actionType": 0,
            "moveResult": {
                "moveId": 14,
                "targetId": "0e32b4a6-7b08-4c4c-a1e5-f7756a4dacf1",
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

</head>
<body>
<h1>SEP3動作説明用簡易ボード</h1>

<div class="flex">
<div class="left">

<p>
<b>アドレス</b> か <b>値</b> をclick ➡ fromOP (and Clipboard)<br>
<b>値</b>横の <b>[T]</b> を click ➡ toOP
</p>

<table>
    <caption>変数領域</caption>
    <tr><th>変数名</th><th>アドレス<br>番地</th><th>値</th></tr>
    <tr><td><input class="variable" type="text" value="i_a"></span></td><td><span id="_0x0102" class="address" onclick="clipboardCopyBySpanId('_0x0102')">0x0102</span></td><td><input class="variable" id="val_0x0102" value="100" onclick="clipboardCopyByInputId('val_0x0102')"/><button onclick="setToOp('val_0x0102')">T</button></td></tr>
    <tr><td><input class="variable" value="i_b"></span></td><td><span id="_0x0103" class="address" onclick="clipboardCopyBySpanId('_0x0103')">0x0103</span></td><td><input class="variable" id="val_0x0103" value="200" onclick="clipboardCopyByInputId('val_0x0103')"><button onclick="setToOp('val_0x0103')">T</button></td></tr>
    <tr><td><input class="variable" value="i_c"></span></td><td><span id="_0x0104" class="address" onclick="clipboardCopyBySpanId('_0x0104')">0x0104</span></td><td><input class="variable" id="val_0x0104" value="250" onclick="clipboardCopyByInputId('val_0x0104')"><button onclick="setToOp('val_0x0104')">T</button></td></tr>
    <tr><td><input class="variable" value="ip_d"></span></td><td><span id="_0x0105" class="address" onclick="clipboardCopyBySpanId('_0x0105')">0x0105</span></td><td><input class="variable" id="val_0x0105" value="0x0103" onclick="clipboardCopyByInputId('val_0x0105')"><button onclick="setToOp('val_0x0105')">T</button></td></tr>
    <tr><td><input class="variable" value="ip_e"></span></td><td><span id="_0x0106" class="address" onclick="clipboardCopyBySpanId('_0x0106')">0x0106</span></td><td><input class="variable" id="val_0x0106" value="0x0109" onclick="clipboardCopyByInputId('val_0x0106')"><button onclick="setToOp('val_0x0106')">T</button></td></tr>
    <tr><td><input class="variable" value="ia_f[0]"></span></td><td><span id="_0x0107" class="address" onclick="clipboardCopyBySpanId('_0x0107')">0x0107</span></td><td><input class="variable" id="val_0x0107" value="410" onclick="clipboardCopyByInputId('val_0x0107')"><button onclick="setToOp('val_0x0107')">T</button></td></tr>
    <tr><td><input class="variable" value="ia_f[1]"></span></td><td><span id="_0x0108" class="address" onclick="clipboardCopyBySpanId('_0x0108')">0x0108</span></td><td><input class="variable" id="val_0x0108" value="440" onclick="clipboardCopyByInputId('val_0x0108')"><button onclick="setToOp('val_0x0108')">T</button></td></tr>
    <tr><td><input class="variable" value="ia_f[2]"></span></td><td><span id="_0x0109" class="address" onclick="clipboardCopyBySpanId('_0x0109')">0x0109</span></td><td><input class="variable" id="val_0x0109" value="210" onclick="clipboardCopyByInputId('val_0x0109')"><button onclick="setToOp('val_0x0109')">T</button></td></tr>
    <tr><td><input class="variable" value="ia_f[3]"></span></td><td><span id="_0x010a" class="address" onclick="clipboardCopyBySpanId('_0x010a')">0x010a</span></td><td><input class="variable" id="val_0x010a" value="550" onclick="clipboardCopyByInputId('val_0x010a')"><button onclick="setToOp('val_0x010a')">T</button></td></tr>
    <tr><td><input class="variable" value="ipa_g[0]"></span></td><td><span id="_0x010b" class="address" onclick="clipboardCopyBySpanId('_0x010b')">0x010b</span></td><td><input class="variable" id="val_0x010b" value="0x0102" onclick="clipboardCopyByInputId('val_0x010b')"><button onclick="setToOp('val_0x010b')">T</button></td></tr>
    <tr><td><input class="variable" value="ipa_g[1]"></span></td><td><span id="_0x010c" class="address" onclick="clipboardCopyBySpanId('_0x010c')">0x010c</span></td><td><input class="variable" id="val_0x010c" value="0x0103" onclick="clipboardCopyByInputId('val_0x010c')"><button onclick="setToOp('val_0x010c')">T</button></td></tr>
    <tr><td><input class="variable" value="ipa_g[2]"></span></td><td><span id="_0x010d" class="address" onclick="clipboardCopyBySpanId('_0x010d')">0x010d</span></td><td><input class="variable" id="val_0x010d" value="0x0104" onclick="clipboardCopyByInputId('val_0x010d')"><button onclick="setToOp('val_0x010d')">T</button></td></tr>
    <tr><td><input class="variable" value="ipa_g[3]"></span></td><td><span id="_0x010e" class="address" onclick="clipboardCopyBySpanId('_0x010e')">0x010e</span></td><td><input class="variable" id="val_0x010e" value="0x0106" onclick="clipboardCopyByInputId('val_0x010e')"><button onclick="setToOp('val_0x010e')">T</button></td></tr>
    <tr><td><input class="variable" value="c_h"></span></td><td><span id="_0x010f" class="address" onclick="clipboardCopyBySpanId('_0x010f')">0x010f</span></td><td><input class="variable" id="val_0x010f" value="400" onclick="clipboardCopyByInputId('val_0x010f')"><button onclick="setToOp('val_0x010f')">T</button></td></tr>
</table>
<br>

</div>
<div class="left">
<p>
<b>値</b> を click ➡ fromOP (and Clipboard)<br>
<b>値</b>横の <b>[T]</b> を click ➡ toOP
</p>

<table>
    <caption>スタック領域</caption>
    <tr><th>R6<br>	(SP)</th><th>アドレス<br>番地</th><th>値</th></tr>
    <tr><td class="R6radio"><input name="sp" type="radio" id="sp9" value="9" onclick="setSP(9)"></td><td><label for="sp9" class="address">0x1009</label></td><td><input class="variable" id="stack_0x1009" value="0x0000" onclick="clipboardCopyByInputId('stack_0x1009')"><button onclick="setToOp('stack_0x1009')">T</button></td></tr>
    <tr><td class="R6radio"><input name="sp" type="radio" id="sp8" value="8" onclick="setSP(8)"></td><td><label for="sp8" class="address">0x1008</label></td><td><input class="variable" id="stack_0x1008" value="0x0000" onclick="clipboardCopyByInputId('stack_0x1008')"><button onclick="setToOp('stack_0x1008')">T</button></td></tr>
    <tr><td class="R6radio"><input name="sp" type="radio" id="sp7" value="7" onclick="setSP(7)"></td><td><label for="sp7" class="address">0x1007</label></td><td><input class="variable" id="stack_0x1007" value="0x0000" onclick="clipboardCopyByInputId('stack_0x1007')"><button onclick="setToOp('stack_0x1007')">T</button></td></tr>
    <tr><td class="R6radio"><input name="sp" type="radio" id="sp6" value="6" onclick="setSP(6)"></td><td><label for="sp6" class="address">0x1006</label></td><td><input class="variable" id="stack_0x1006" value="0x0000" onclick="clipboardCopyByInputId('stack_0x1006')"><button onclick="setToOp('stack_0x1006')">T</button></td></tr>
    <tr><td class="R6radio"><input name="sp" type="radio" id="sp5" value="5" onclick="setSP(5)"></td><td><label for="sp5" class="address">0x1005</label></td><td><input class="variable" id="stack_0x1005" value="0x0000" onclick="clipboardCopyByInputId('stack_0x1005')"><button onclick="setToOp('stack_0x1005')">T</button></td></tr>
    <tr><td class="R6radio"><input name="sp" type="radio" id="sp4" value="4" onclick="setSP(4)"></td><td><label for="sp4" class="address">0x1004</label></td><td><input class="variable" id="stack_0x1004" value="0x0000" onclick="clipboardCopyByInputId('stack_0x1004')"><button onclick="setToOp('stack_0x1004')">T</button></td></tr>
    <tr><td class="R6radio"><input name="sp" type="radio" id="sp3" value="3" onclick="setSP(3)"></td><td><label for="sp3" class="address">0x1003</label></td><td><input class="variable" id="stack_0x1003" value="0x0000" onclick="clipboardCopyByInputId('stack_0x1003')"><button onclick="setToOp('stack_0x1003')">T</button></td></tr>
    <tr><td class="R6radio"><input name="sp" type="radio" id="sp2" value="2" onclick="setSP(2)"></td><td><label for="sp2" class="address">0x1002</label></td><td><input class="variable" id="stack_0x1002" value="0x0000" onclick="clipboardCopyByInputId('stack_0x1002')"><button onclick="setToOp('stack_0x1002')">T</button></td></tr>
    <tr><td class="R6radio"><input name="sp" type="radio" id="sp1" value="1" onclick="setSP(1)"></td><td><label for="sp1" class="address">0x1001</label></td><td><input class="variable" id="stack_0x1001" value="0x0000" onclick="clipboardCopyByInputId('stack_0x1001')"><button onclick="setToOp('stack_0x1001')">T</button></td></tr>
    <tr><td class="R6radio"><input name="sp" type="radio" id="sp0" value="0" onclick="setSP(0)" checked></td><td><label for="sp0" class="address">0x1000</label></td><td><input class="variable" id="stack_0x1000" value="0x0000" onclick="clipboardCopyByInputId('stack_0x1000')"><button onclick="setToOp('stack_0x1000')">T</button></td></tr>
    <tr><td>　</td></tr>
    <tr><td>Mapped<br> I/O</td><td><span id="_0xffe0" onclick="clipboardCopyBySpanId('_0xffe0')" class="address">0xffe0</span></td><td><input class="variable" id="val_0xffe0" value="0x1111" onclick="clipboardCopyByInputId('val_0xffe0')"><button onclick="setToOp('val_0xffe0')">T</button></td></tr>
</table>

</div>
<div class="middle">
<!--
    好きな麺類：<input type="text"  list="Noodles" id="FavoriteNoodles">
    <datalist id="Noodles">
      <option value="ラーメン"></option>
      <option value="日本そば"></option>
      <option value="つけ麺"></option>
      <option value="油そば"></option>
      <option value="焼きそば"></option>
    </datalist>
-->

<table>
<caption>MOV (push, pop も含む), ADD, SUB</caption>
<tr><th>命令</th><th>FromOP</th><th>ToOP</th></tr>
<!--<tr><td></td>
<td>
<select class="variable" id="fromOriginal">
<option value="">選択！</option>
<optgroup label="直接">
<option value="##">即値</option>
<optgroup label="直接">
<option value="R0">R0</option>
<option value="R1">R1</option>
<option value="R2">R2</option>
<option value="R3">R3</option>
<option value="R4">R4</option>
<option value="R6">R6</option>
<option value="R7">R7</option>
<optgroup label="間接">
<option value="(R0)">(R0)</option>
<option value="(R1)">(R1)</option>
<option value="(R2)">(R2)</option>
<option value="(R3)">(R3)</option>
<option value="(R4)">(R4)</option>
<option value="(R6)">(R6)</option>
<option value="(R7)">(R7)</option>
<optgroup label="-間接">
<option value="-(R0)">-(R0)</option>
<option value="-(R1)">-(R1)</option>
<option value="-(R2)">-(R2)</option>
<option value="-(R3)">-(R3)</option>
<option value="-(R4)">-(R4)</option>
<option value="-(R6)">-(R6)</option>
<option value="-(R7)">-(R7)</option>
</select>
<button onclick="setFromOriginal()">反映</button>
,</td>
<td>
<select class="variable3" id="toOriginal" oninput="setToOriginal()">
<option value="">対象を選択！</option>
<optgroup label="直接">
<option value="R0">R0</option>
<option value="R1">R1</option>
<option value="R2">R2</option>
<option value="R3">R3</option>
<option value="R4">R4</option>
<option value="R6">R6</option>
<option value="R7">R7</option>
<optgroup label="間接">
<option value="(R0)">(R0)</option>
<option value="(R1)">(R1)</option>
<option value="(R2)">(R2)</option>
<option value="(R3)">(R3)</option>
<option value="(R4)">(R4)</option>
<option value="(R6)">(R6)</option>
<option value="(R7)">(R7)</option>
<optgroup label="-間接">
<option value="-(R0)">-(R0)</option>
<option value="-(R1)">-(R1)</option>
<option value="-(R2)">-(R2)</option>
<option value="-(R3)">-(R3)</option>
<option value="-(R4)">-(R4)</option>
<option value="-(R6)">-(R6)</option>
<option value="-(R7)">-(R7)</option>
<optgroup label="間接+">
<option value="(R0)+">(R0)+</option>
<option value="(R1)+">(R1)+</option>
<option value="(R2)+">(R2)+</option>
<option value="(R3)+">(R3)+</option>
<option value="(R4)+">(R4)+</option>
<option value="(R6)+">(R6)+</option>
<option value="(R7)+">(R7)+</option>
</select>
</td>
</tr>-->
<tr><td><td><input class="variable2" id="fromOP" oninput="setFOValueByInput()" />,</td><td>
<select class="variable3" id="writeID" oninput="setTOValueByInput()">
<option value="">対象を選択！</option>
<optgroup label="レジスタ">
<option value="R0">R0</option>
<option value="R1">R1</option>
<option value="R2">R2</option>
<option value="R3">R3</option>
<option value="R4">R4(SP)</option>
<option value="R6">R6(SP)</option>
<option value="R7">R7(PC)</option>
<optgroup label="間接参照(変数)">
<option value="val_0x0102">(0x0102) ;i_a</option>
<option value="val_0x0103">(0x0103) ;i_b</option>
<option value="val_0x0104">(0x0104) ;i_c</option>
<option value="val_0x0105">(0x0105) ;ip_d</option>
<option value="val_0x0106">(0x0106) ;ip_e</option>
<option value="val_0x0107">(0x0107) ;ia_f[0]</option>
<option value="val_0x0108">(0x0108) ;ia_f[1]</option>
<option value="val_0x0109">(0x0109) ;ia_f[2]</option>
<option value="val_0x010a">(0x010a) ;ia_f[3]</option>
<option value="val_0x010b">(0x010b) ;ipa_g[0]</option>
<option value="val_0x010c">(0x010c) ;ipa_g[1]</option>
<option value="val_0x010d">(0x010d) ;ipa_g[2]</option>
<option value="val_0x010e">(0x010e) ;ipa_g[3]</option>
<option value="val_0x010f">(0x010f) ;c_h</option>
<optgroup label="間接参照(スタック)">
<option value="stack_0x1009">(0x1009)</option>
<option value="stack_0x1008">(0x1008)</option>
<option value="stack_0x1007">(0x1007)</option>
<option value="stack_0x1006">(0x1006)</option>
<option value="stack_0x1005">(0x1005)</option>
<option value="stack_0x1004">(0x1004)</option>
<option value="stack_0x1003">(0x1003)</option>
<option value="stack_0x1002">(0x1002)</option>
<option value="stack_0x1001">(0x1001)</option>
<option value="stack_0x1009">(0x1000)</option>
<optgroup label="Maped I/O">
<option value="val_0xffe0">(0xffe0)</option>
</select>

</td></tr>
<tr><td><button class="run" onclick="pushLastClickValue()">push</button></td><td><input class="variable2" id="pushFO" disabled/></td><td></td><td></td></tr>
<tr><td><button class="run" onclick="popLastClickValue()">pop</button></td><td></td><td><input class="variable3" id="popTO" disabled /></td></tr>
<tr><td><button class="run" onclick="movFromTo()">MOV</button>　</td><td><input class="variable2" id="movFO" disabled/>,</td><td><input class="variable3" id="movTO"/ disabled></td></tr>
<tr><td><button class="run" onclick="addFromTo()">ADD</button>　</td><td><input class="variable2" id="addFO" disabled/>,</td><td><input class="variable3" id="addTO" disabled/></td></tr>
<tr><td><button class="run" onclick="subFromTo()">SUB</button>　</td><td><input class="variable2" id="subFO" disabled/>,</td><td><input class="variable3" id="subTO" disabled/></td></tr>
<tr><td><button class="run" onclick="xorFromTo()">XOR</button>　</td><td><input class="variable2" id="xorFO" disabled/>,</td><td><input class="variable3" id="xorTO" disabled/></td></tr>
<tr><td><button class="run" onclick="clrTo()">CLR</button>　</td><td></td><td><input class="variable3" id="clrTO" disabled/></td></tr>
</table>
<p>
(特殊) FromOp に <button onclick="preDecrementPop()">pop</button> (??? -(R6),?? 用) </p>
<br>

<p>
<b>値</b> を click ➡ fromOP (and Clipboard)<br>
<b>値</b>横の <b>[T]</b> を click ➡ toOP
</p>

<table>
    <caption>レジスタの値</caption>
    <tr><th>レジスタ名</th><th>値</th><th></th></tr>
    <tr><td class="reg">R0</td><td><input class="variable2" id="R0" value="0x0000" onclick="clipboardCopyByInputId('R0')"></td><td><button name="push" value="push" onclick="push('R0');">push</button><button name="pop" value="pop" onclick="pop('R0');">pop</button><button onclick="setToOp('R0')">T</button></td></td></tr>
    <tr><td class="reg">R1</td><td><input class="variable2" id="R1" value="0x0000" onclick="clipboardCopyByInputId('R1')"></td><td><button name="push" value="push" onclick="push('R1');">push</button><button name="pop" value="pop" onclick="pop('R1');">pop</button><button onclick="setToOp('R1')">T</button></td></tr>
    <tr><td class="reg">R2</td><td><input class="variable2" id="R2" value="0x0000" onclick="clipboardCopyByInputId('R2')"></td><td><button name="push" value="push" onclick="push('R2');">push</button><button name="pop" value="pop" onclick="pop('R2');">pop</button><button onclick="setToOp('R2')">T</button></td></tr>
    <tr><td class="reg">R3</td><td><input class="variable2" id="R3" value="0x0000" onclick="clipboardCopyByInputId('R3')"></td><td><button name="push" value="push" onclick="push('R3');">push</button><button name="pop" value="pop" onclick="pop('R3');">pop</button><button onclick="setToOp('R3')">T</button></td></tr>
    <tr><td class="reg">R4(FP)</td><td><input class="variable2" id="R4" value="0x0000" onclick="clipboardCopyByInputId('R4')"></td><td><button name="push" value="push" onclick="push('R4');">push</button><button name="pop" value="pop" onclick="pop('R4');">pop</button><button onclick="setToOp('R4')">T</button></td></tr>
<!--    <tr><td>R5(PSW)</td><td><input id="R5" text=""></td></tr>-->
    <tr><td class="reg">R6(SP)</td><td><input class="variable2" id="R6" value="0x1000" onclick="clipboardCopyByInputId('R6')" readonly></td><td><button onclick="setToOp('R6')">T</button></td></tr>
    <tr><td class="reg">R7(PC)</td><td><input class="variable2" id="R7" value="0x0100" onclick="clipboardCopyByInputId('R7')"></td><td><button name="push" value="push" onclick="push('R7');">push</button><button name="pop" value="pop" onclick="pop('R7');">pop</button><button onclick="setToOp('R7')">T</button></td></tr>
</table>
<br>

<!--
<table>
    <caption>命令</caption>
    <tr><th>命令</th><th>From</th><th></th><th>To</th><th>実行</th></tr>
    <tr><td>MOV</td><td>　
即値　<input id="MOV_IMMEDIATE" text=""></td><td>,</td><td>直接　
<select id="MOV_IMMEDIATE_TODIRECT">
<option value="">RO</option>
<option value="">R1</option>
<option value="">R2</option>
<option value="">R3</option>
<option value="">R4</option>
</select>
</td><td><button name="push" onclick="movDirect();">run</button></td></tr>
</table>
<br>
<br>
<br>
-->

<table>
    <caption>PSWの値</caption>
    <tr><th>N</th><th>Z</th><th>V</th><th>C</th></tr>
    <tr>
<td><select id="PSW_N"><option>1</option><option selected>0</option></select></td>
<td><select id="PSW_Z"><option>1</option><option selected>0</option></select></td>
<td><select id="PSW_V"><option>1</option><option selected>0</option></select></td>
<td><select id="PSW_C"><option>1</option><option selected>0</option></select></td>
    </tr>
</table>

</div>
<div class="right">
<select id="selectText">
<option value="cv00" selected>空欄</option>
<option value="cv03">cv03</option>
<option value="cv04">cv04</option>
</select>
<input type="button" onclick="updateText();" value="setText">
<button class="run" onclick="window.location.reload(); false;">RELOAD</button><br>
<textarea id="freeArea" class="freeArea" width="400" height="500">
</textarea>
</div>
</div>

<script type="application/javascript">
sp=0;

function push(str){
  rg = document.getElementById(str);
  push0(rg.value);
}

function pop(str){
  if (0>=sp) {
    alert("sp が 0 なので pop できません");
    return;
  }
  val = pop0();
  rg = document.getElementById(str);
  rg.value = val;
}

function clipboardCopy(id,str) {
  navigator.clipboard.writeText(str);
}

function setFOValue(val){
  inp=document.getElementById("fromOP")
  inp.value = val;
  inp=document.getElementById("pushFO")
  inp.value = val;
  inp=document.getElementById("movFO")
  inp.value = val;
  inp=document.getElementById("addFO")
  inp.value = val;
  inp=document.getElementById("subFO")
  inp.value = val;
  inp=document.getElementById("xorFO")
  inp.value = val;
}

function setFOValueByInput(){
  inp=document.getElementById("fromOP")
  val = inp.value;
  inp=document.getElementById("pushFO")
  inp.value = val;
  inp=document.getElementById("movFO")
  inp.value = val;
  inp=document.getElementById("addFO")
  inp.value = val;
  inp=document.getElementById("subFO")
  inp.value = val;
  inp=document.getElementById("xorFO")
  inp.value = val;
}

function setTOValue(val){
  inp=document.getElementById("writeID")
  inp.value = val;
  inp=document.getElementById("popTO")
  inp.value = val;
  inp=document.getElementById("movTO")
  inp.value = val;
  inp=document.getElementById("addTO")
  inp.value = val;
  inp=document.getElementById("subTO")
  inp.value = val;
  inp=document.getElementById("clrTO")
  inp.value = val;
  inp=document.getElementById("xorTO")
  inp.value = val;
}

function setTOValueByInput(){
  inp=document.getElementById("writeID")
  val = inp.value;
  inp=document.getElementById("popTO")
  inp.value = val;
  inp=document.getElementById("movTO")
  inp.value = val;
  inp=document.getElementById("addTO")
  inp.value = val;
  inp=document.getElementById("subTO")
  inp.value = val;
  inp=document.getElementById("clrTO")
  inp.value = val;
}

function clipboardCopyByInputId(str) {
  lastType = "input";
  lastCopyId = str
  tar = document.getElementById(str);
  lastCopyTarget = tar
  navigator.clipboard.writeText(tar.value);
  setFOValue(tar.value);
}

function clipboardCopyBySpanId(str) {
  lastType = "span";
  lastCopyId = str
  tar = document.getElementById(str);
  lastCopyTarget = tar
  navigator.clipboard.writeText(tar.innerHTML);
  setFOValue(tar.innerHTML);
}

function getStackTarget(){
  stackLabel = "stack_0x100"+sp;
  return document.getElementById(stackLabel);
}

function push0(val) {
  st = getStackTarget()
  st.value = val;
  sp++;
  setSP(sp);
}

function pop0() {
  sp--;
  setSP(sp);
  st = getStackTarget()
  return st.value;
}

function setSP(val) {
  sp = val;
  r6 = document.getElementById("R6");
  r6val = val + parseInt("1000",16);
  r6.value = "0x" + (('0000' + r6val.toString(16).toUpperCase()).substr(-4));
  rg = document.getElementById('sp'+sp);
  rg.checked = true;  
}

function getFromOpValue() {
  fromOP = document.getElementById("fromOP");
  return fromOP.value;
}

function setFromOpValue(val) {
  fromOP = document.getElementById("fromOP");
  fromOP.value = val;
}

function pushLastClickValue() {
  push0(getFromOpValue());
}

function popLastClickValue() {
  if (0>=sp) {
    alert("sp が 0 なので pop できません");
    return;
  }
  val=pop0();
  ta = getWriteTarget();
  ta.value = val;
}

function preDecrementPop() {
  if (0>=sp) {
    alert("sp が 0 なので pop できません");
    return;
  }
  val=pop0();
  setFromOpValue(val);
}

function getWriteID() {
  wi = document.getElementById("writeID");
  return wi.value;
}

function getWriteTarget() {
  tar = document.getElementById(getWriteID());
  return tar;
}

function setWriteID(val) {
  setTOValue(val);
}

function setToOp(str) {
  lastToOpTarget = document.getElementById(str);
  setWriteID(str);
}

function movFromTo() {
  fromOP = document.getElementById("fromOP");
  ta = getWriteTarget();
  ta.value = fromOP.value;
  val = parseInt(ta.value);
  resetN();
  resetZ();
  resetV();
  if (val==0) { setZ(); }
  if (val<0 || val>=32768) { setN(); }
}

function clrTo() {
  ta = getWriteTarget();
  ta.value = "0x0000";
  resetN();
  setZ();
  resetV();
  resetC();
}

function addFromTo() {
  fromOP = document.getElementById("fromOP");
  ta = getWriteTarget();
  ta.value = add16(ta.value, fromOP.value);
  val = parseInt(ta.value);
  if (val <0 || val >= 32768) {
    setN();
  } 
  if (val==0){
    setZ();
  } 
  if (ta.id == "R6") {
    setSP(sp+parseInt(fromOP.value));
  }
}

function subFromTo() {
  fromOP = document.getElementById("fromOP");
  ta = getWriteTarget();
  ta.value = sub16(ta.value, fromOP.value);
  val = parseInt(ta.value);
  if (val <0 || val >= 32768) {
    setN();
  } 
  if (val==0){
    setZ();
  } 
  if (ta.id == "R6") {
    setSP(sp-parseInt(fromOP.value));
  }
}

function xorFromTo() {
  fromOP = document.getElementById("fromOP");
  ta = getWriteTarget();
  ta.value = xor16(ta.value, fromOP.value);
  val = parseInt(ta.value);
  if (val <0 || val >= 32768) {
    setN();
  } 
  if (val==0){
    setZ();
  } 
  if (ta.id == "R6") {
    setSP(sp-parseInt(fromOP.value));
  }
}

function isHex(a) {
  if (a.length >= 3) {
    if (a.slice(0,2) == "0x") {
      return true;
    }
  }
  return false;
}

function hexValue(a){
  
}

function add16(a, b) {
  mode = 10;
  if (isHex(a)){
    mode = 16;
    aVal = parseInt(a);
  } else {
    aVal = parseInt(a);
  }
  if (isHex(b)){
    mode = 16;
    bVal = parseInt(b);
  } else {
    bVal = parseInt(b);
  }
  val = aVal+bVal;
  if (val > 65535) {
    val -= 65536;
  }
  if (mode==10){
    return val;
  } else {
    if (0 > val) {
      val += 65536;
    }
    return "0x" + (('0000' + val.toString(16).toUpperCase()).substr(-4));
  }
}

function sub16(a, b) {
  mode = 10;
  if (isHex(a)){
    mode = 16;
    aVal = parseInt(a);
  } else {
    aVal = parseInt(a);
  }
  if (isHex(b)){
    mode = 16;
    bVal = parseInt(b);
  } else {
    bVal = parseInt(b);
  }
  val = aVal - bVal;
  if (val > 65535) {
    val -= 65536;
  }
  if (mode==10){
    return aVal-bVal;
  } else {
    if (0 > val) {
      val += 65536;
    }
    return "0x" + (('0000' + val.toString(16).toUpperCase()).substr(-4));
  }
}

function xor16(a, b) {
  mode = 10;
  if (isHex(a)){
    mode = 16;
    aVal = parseInt(a);
  } else {
    aVal = parseInt(a);
  }
  if (isHex(b)){
    mode = 16;
    bVal = parseInt(b);
  } else {
    bVal = parseInt(b);
  }
  val = aVal ^ bVal;
  if (val > 65535) {
    val -= 65536;
  }
  if (mode==10){
    return aVal ^ bVal;
  } else {
    if (0 > val) {
      val += 65536;
    }
    return "0x" + (('0000' + val.toString(16).toUpperCase()).substr(-4));
  }
}

function setN(){
  nn=document.getElementById("PSW_N");
  nn.value=1;
}

function resetN(){
  nn=document.getElementById("PSW_N");
  nn.value=0;
}

function setZ(){
  nz=document.getElementById("PSW_Z");
  nz.value=1;
}

function resetZ(){
  nz=document.getElementById("PSW_Z");
  nz.value=0;
}

function setV(){
  nv=document.getElementById("PSW_V");
  nv.value=1;
}

function resetV(){
  nv=document.getElementById("PSW_V");
  nv.value=0;
}

function setC(){
  nc=document.getElementById("PSW_C");
  nc.value=1;
}

function resetC(){
  nc=document.getElementById("PSW_C");
  nc.value=0;
}

function setFromOriginal(){
  ta = document.getElementById("fromOriginal");
  console.log("ta.value=["+ta.value+"]");
  if (ta.value.length == 2){
    console.log("ta.value=["+ta.value+"]");
    clipboardCopyByInputId(ta.value);
  } else if (ta.value.length == 4){
    val = ta.value.substr(1,2);
    console.log("val=["+val+"]");
    rg = document.getElementById(val);
    val = rg.value;
    console.log("val=["+val+"]");
    if (val.substr(0,3) == "0x0") { //変数領域
      ta="val_"+val;
      console.log("val_id=["+ta+"]");
      rg = document.getElementById(ta);
      if (rg != null) {
        clipboardCopyByInputId(ta);
      } else {
        console.log("getElementById("+ta+") is null.");
      }
    } else if (val.substr(0,3) == "0x1") { //スタック領域
      ta="stack_"+val;
      console.log("stack_id=["+ta+"]");
      rg = document.getElementById(ta);
      if (rg != null) {
        clipboardCopyByInputId(rg);
      } else {
        console.log("getElementById("+ta+") is null.");
      }
    }
  }
}

function updateText() {
    ta = document.getElementById("freeArea");
    st = document.getElementById("selectText");
    if (st.value == "cv00") {
        ta.value="";
    } else if (st.value == "cv03") {
ta.value = 
`1+2*3


1FA   MOV #1,(R6)+
1FC   MOV #2,(R6)+
1FE   MOV #3,(R6)+
200   JSR MUL
202   SUB #2,R6
204   MOV R0,(R6)+
205   MOV -(R6),R0
206   MOV -(R6),R1
207   ADD R1,R0
208   MOV R0,(R6)+

400 MUL:
    MOV #6,R0
410 RET

(1) JSR MUL のする仕事は2つあります 
   (1-1) ___ここで何をしますか？___
   (1-2) MUL(400) を PC に代入する

(2) RET は何をしますか？


`;
    } else if (st.value == "cv04") {
ta.value = 
`0100        .= 0x0100
0100        JMP __START
0102    i_a: .word 100
0103    i_b: .word 200
0104    i_c: .word 250
0105    ip_d: .word 0x0103
0106    ip_e: .word 0x0109
0107    ia_f: .blkw 4
010B    ipa_g: .blkw 4
010F    c_h: .word 400
0110    __START:
`;
    }
}

</script>


</body>
</html>
```

## ドキュメント

- 特に以下のドキュメントをよく確認して作業をしてください.
  - `/mnt/c/Users/cs23017/Shizuoka University/ドキュメント/dev/01_poke_clone-v3/Docs/Claude.md`
  - `/mnt/c/Users/cs23017/Shizuoka University/ドキュメント/dev/01_poke_clone-v3/Docs/UML/クラス図.drawio`
  - `/mnt/c/Users/cs23017/Shizuoka University/ドキュメント/dev/01_poke_clone-v3/Docs/UML/シーケンス図.drawio`
  - `/mnt/c/Users/cs23017/Shizuoka University/ドキュメント/dev/01_poke_clone-v3/Docs/UML/アーキテクチャ構成.drawio`
  - `/mnt/c/Users/cs23017/Shizuoka University/ドキュメント/dev/01_poke_clone-v3/Docs/フロントエンド実装手順.md`
  - `/mnt/c/Users/cs23017/Shizuoka University/ドキュメント/dev/01_poke_clone-v3/Docs/要件定義.md` 