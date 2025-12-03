let connection = null;
let battleState = null;
let playerId = null;

$(document).ready(function () {
    $('#connect-button').on('click', connectToBattle);

    // Move buttons
    for (let i = 0; i < 4; i++) {
        $(`#move-${i}`).on('click', function () {
            submitMoveAction(i);
        });
    }

    // Action buttons
    $('#catch-button').on('click', submitCatchAction);
    $('#escape-button').on('click', submitEscapeAction);
});

async function getAuthToken() {
    const apiUrl = $('#api-url').val();
    try {
        const response = await fetch(`${apiUrl}/api/Auth/login/mock`, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify({
                username: 'testplayer1',
                password: 'testpassword'
            })
        });

        const data = await response.json();
        return data.token;
    } catch (error) {
        console.error('Failed to get auth token:', error);
        throw error;
    }
}

async function connectToBattle() {
    const apiUrl = $('#api-url').val();
    const battleId = $('#battle-id').val();
    playerId = $('#player-id').val();

    if (!battleId || !playerId) {
        addLog('Battle IDとPlayer IDを入力してください', 'info');
        return;
    }

    try {
        // JWTトークンを取得
        const token = await getAuthToken();

        // SignalR接続の作成
        connection = new signalR.HubConnectionBuilder()
            .withUrl(`${apiUrl}/battlehub`, {
                accessTokenFactory: () => token
            })
            .withAutomaticReconnect()
            .configureLogging(signalR.LogLevel.Information)
            .build();

        // イベントハンドラの設定
        connection.on("BattleStarted", onBattleStarted);
        connection.on("ReceiveTurnResult", onReceiveTurnResult);
        connection.on("BattleEnded", onBattleEnded);
        connection.on("BattleClosed", onBattleClosed);
        connection.on("Error", onError);

        // 接続
        await connection.start();
        $('#connection-status').text('接続中...').removeClass('disconnected').addClass('connected');

        // バトルに参加
        await connection.invoke("JoinBattle", battleId);

    } catch (err) {
        console.error(err);
        $('#connection-status').text('接続失敗').removeClass('connected').addClass('disconnected');
        addLog(`接続エラー: ${err.message}`, 'info');
    }
}

function onBattleStarted(state) {
    console.log("Battle started:", state);
    battleState = state;
    updateUI();
    $('#connection-status').text('接続済み').addClass('connected');
    addLog('バトルが開始されました！', 'info');
}

function onReceiveTurnResult(result) {
    console.log("Turn result:", result);

    // アクション結果をログに追加
    result.actionResults.forEach(actionResult => {
        if (actionResult.moveResult) {
            const moveResult = actionResult.moveResult;
            if (moveResult.isSuccess) {
                const damage = moveResult.damage;
                const isCritical = moveResult.hitContext?.isCritical;
                const effectiveness = moveResult.hitContext?.typeEffectiveness;

                let message = `${damage}ダメージ！`;
                if (isCritical) message += ' 急所に当たった！';
                if (effectiveness > 1) message += ' 効果は抜群だ！';
                if (effectiveness < 1) message += ' 効果はいまひとつのようだ...';

                addLog(message, 'damage');

                // HP更新（仮）
                updateHPBars();
            } else {
                addLog(`攻撃は外れた！${moveResult.failureReason}`, 'info');
            }
        }

        if (actionResult.catchResult) {
            if (actionResult.catchResult.isSuccess) {
                addLog('捕獲に成功しました！', 'heal');
            } else {
                addLog('捕獲に失敗しました...', 'info');
            }
        }

        if (actionResult.escapeResult) {
            if (actionResult.escapeResult.isSuccess) {
                addLog('逃走に成功しました！', 'heal');
            } else {
                addLog(`逃走に失敗: ${actionResult.escapeResult.failureReason}`, 'info');
            }
        }
    });

    // UI更新
    setTimeout(() => {
        updateUI();
    }, 500);
}

function onBattleEnded(result) {
    console.log("Battle ended:", result);

    if (result === "Caught") {
        addLog('野生のポケモンを捕獲しました！', 'heal');
    } else if (result === "Escaped") {
        addLog('バトルから逃走しました', 'info');
    } else {
        addLog(`バトル終了！ 勝者: ${result}`, 'info');
    }

    disableAllButtons();
}

function onBattleClosed() {
    addLog('バトルが終了し、接続が切断されました', 'info');
    $('#connection-status').text('切断').removeClass('connected').addClass('disconnected');
    connection = null;
}

function onError(message) {
    console.error("Error:", message);
    addLog(`エラー: ${message}`, 'damage');
}

async function submitMoveAction(moveIndex) {
    if (!connection || !playerId || !battleState) return;

    try {
        // Get actual moveId from battleState.pokemonEntities
        const playerState = battleState.player1.playerId === playerId ? battleState.player1 : battleState.player2;
        const playerPokemonEntity = playerState.pokemonEntities[playerState.activePokemonIndex];

        console.log('DEBUG: playerPokemonEntity:', playerPokemonEntity);
        console.log('DEBUG: moves:', playerPokemonEntity.moves);
        console.log('DEBUG: moveIndex:', moveIndex);

        if (!playerPokemonEntity.moves || moveIndex >= playerPokemonEntity.moves.length) {
            addLog(`技${moveIndex + 1}が見つかりません`, 'damage');
            return;
        }

        const move = playerPokemonEntity.moves[moveIndex];
        console.log('DEBUG: selected move:', move);
        console.log('DEBUG: sending moveId:', move.moveId);

        await connection.invoke("SubmitAction", $('#battle-id').val(), {
            playerId: playerId,
            actionType: 0, // Attack
            value: move.moveId
        });

        addLog(`${move.name}を使用！`, 'info');
        disableAllButtons();
    } catch (err) {
        console.error(err);
        addLog(`アクション送信エラー: ${err.message}`, 'damage');
    }
}

async function submitCatchAction() {
    if (!connection || !playerId) return;

    try {
        await connection.invoke("SubmitAction", $('#battle-id').val(), {
            playerId: playerId,
            actionType: 2, // Catch
            value: 0
        });

        addLog('捕獲を試みます...', 'info');
        disableAllButtons();
    } catch (err) {
        console.error(err);
        addLog(`捕獲エラー: ${err.message}`, 'damage');
    }
}

async function submitEscapeAction() {
    if (!connection || !playerId) return;

    try {
        await connection.invoke("SubmitAction", $('#battle-id').val(), {
            playerId: playerId,
            actionType: 3, // Escape
            value: 0
        });

        addLog('逃走を試みます...', 'info');
        disableAllButtons();
    } catch (err) {
        console.error(err);
        addLog(`逃走エラー: ${err.message}`, 'damage');
    }
}

function updateUI() {
    if (!battleState) return;

    const playerState = battleState.player1.playerId === playerId ? battleState.player1 : battleState.player2;
    const opponentState = battleState.player1.playerId === playerId ? battleState.player2 : battleState.player1;

    // プレイヤーのポケモン情報を更新（pokemonEntitiesから取得）
    const playerPokemonEntity = playerState.pokemonEntities[playerState.activePokemonIndex];
    const playerPokemonState = playerState.party[playerState.activePokemonIndex];

    $('#player-name').text(playerPokemonEntity.species.name);
    $('#player-level').text(playerPokemonEntity.level);
    $('#player-hp-current').text(playerPokemonState.currentHp);
    $('#player-hp-max').text(playerPokemonState.maxHp);
    updateHPBar('#player-hp-bar', playerPokemonState.currentHp, playerPokemonState.maxHp);

    // 相手のポケモン情報を更新（pokemonEntitiesから取得）
    const opponentPokemonEntity = opponentState.pokemonEntities[opponentState.activePokemonIndex];
    const opponentPokemonState = opponentState.party[opponentState.activePokemonIndex];

    $('#opponent-name').text(opponentPokemonEntity.species.name);
    $('#opponent-level').text(opponentPokemonEntity.level);
    $('#opponent-hp-current').text(opponentPokemonState.currentHp);
    $('#opponent-hp-max').text(opponentPokemonState.maxHp);
    updateHPBar('#opponent-hp-bar', opponentPokemonState.currentHp, opponentPokemonState.maxHp);

    // 技ボタンのラベルを更新
    updateMoveButtons(playerPokemonEntity.moves);

    // ボタンを有効化
    enableActionButtons();
}

function updateHPBar(selector, current, max) {
    const percentage = (current / max) * 100;
    const $bar = $(selector);

    $bar.css('width', `${percentage}%`);

    // HP残量に応じて色を変更
    $bar.removeClass('low critical');
    if (percentage <= 25) {
        $bar.addClass('critical');
    } else if (percentage <= 50) {
        $bar.addClass('low');
    }
}

function updateHPBars() {
    // 簡易実装: 実際のHPはサーバーから送られてくるBattleStateを再取得する必要がある
    // ここでは表示のみ
}

function updateMoveButtons(moves) {
    for (let i = 0; i < 4; i++) {
        const $button = $(`#move-${i}`);
        if (i < moves.length) {
            $button.text(moves[i].name);
            $button.show();
        } else {
            $button.text(`技${i + 1}`);
            $button.hide();
        }
    }
}

function enableActionButtons() {
    for (let i = 0; i < 4; i++) {
        $(`#move-${i}`).prop('disabled', false);
    }
    $('#catch-button').prop('disabled', false);
    $('#escape-button').prop('disabled', false);
}

function disableAllButtons() {
    for (let i = 0; i < 4; i++) {
        $(`#move-${i}`).prop('disabled', true);
    }
    $('#catch-button').prop('disabled', true);
    $('#escape-button').prop('disabled', true);
}

function addLog(message, type = 'info') {
    const $log = $('#battle-log');
    const $entry = $('<div>').addClass('log-entry').addClass(type).text(message);
    $log.append($entry);
    $log.scrollTop($log[0].scrollHeight);
}
