import type * as signalR from '@microsoft/signalr';
import { useEffect, useState } from 'react';

import { signalRService } from '@/lib/signalr/connection';
import type { PlayerAction } from '@/types/action';
import type { BattleState, EndResult, ProcessResult } from '@/types/battle';

export const useSignalR = (
    battleId: string,
    token: string,
    onTurnResult: (result: ProcessResult) => void,
    onBattleStarted?: (state: BattleState) => void,
    onBattleEnded?: (endResult: EndResult) => void
) => {
    const [hubConnection, setHubConnection] = useState<signalR.HubConnection | null>(null);
    const [isConnected, setIsConnected] = useState(false);
    const [error, setError] = useState<string | null>(null);

    useEffect(() => {
        const setupConnection = async () => {
            try {
                const connection = await signalRService.connect(token);

                // ターン結果受信ハンドラ
                // サーバーから各ターンの処理結果が送信されたときに呼び出される
                connection.on('ReceiveTurnResult', (result: ProcessResult) => {
                    onTurnResult(result);
                });

                // バトル開始受信ハンドラ
                // バトルルームに参加したときや、CPUバトルが作成されたときに呼び出される
                // onBattleStartedがコンポーネントから渡されている場合のみコールバックを実行
                connection.on('BattleStarted', (state: BattleState) => {
                    if (onBattleStarted) {
                        onBattleStarted(state);
                    }
                });

                // バトル終了受信ハンドラ
                // いずれかのプレイヤーの全ポケモンが倒れた、捕獲成功、逃走成功などでバトルが終了したときに呼び出される
                connection.on('BattleEnded', (endResult: EndResult) => {
                    if (onBattleEnded) {
                        onBattleEnded(endResult);
                    }
                });

                // エラー受信ハンドラ
                // サーバー側でエラーが発生したときに呼び出される
                connection.on('Error', (errorMessage: string) => {
                    console.error('SignalR Error:', errorMessage);
                    setError(errorMessage);
                });

                // バトルルームに参加
                await connection.invoke('JoinBattle', battleId);

                setHubConnection(connection);
                setIsConnected(true);
            } catch (err) {
                console.error('SignalR connection error:', err);
                setError(err instanceof Error ? err.message : 'Unknown error');
            }
        };

        setupConnection();

        return () => {
            signalRService.disconnect();
        };
    }, [battleId, token, onTurnResult, onBattleStarted, onBattleEnded]);

    const submitAction = async (action: PlayerAction) => {
        if (!hubConnection) {
            throw new Error('SignalR not connected');
        }

        try {
            await hubConnection.invoke('SubmitAction', battleId, action);
        } catch (err) {
            console.error('Failed to submit action:', err);
            throw err;
        }
    };

    return { hubConnection, isConnected, error, submitAction };
};
