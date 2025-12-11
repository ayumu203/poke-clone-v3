import type * as signalR from '@microsoft/signalr';
import { useEffect, useRef, useState } from 'react';

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

    // コールバックをuseRefで管理して依存配列から除外
    const onTurnResultRef = useRef(onTurnResult);
    const onBattleStartedRef = useRef(onBattleStarted);
    const onBattleEndedRef = useRef(onBattleEnded);

    // コールバックのrefを最新の値に更新
    useEffect(() => {
        onTurnResultRef.current = onTurnResult;
        onBattleStartedRef.current = onBattleStarted;
        onBattleEndedRef.current = onBattleEnded;
    }, [onTurnResult, onBattleStarted, onBattleEnded]);

    useEffect(() => {
        const setupConnection = async () => {
            try {
                const connection = await signalRService.connect(token);

                // ターン結果受信ハンドラ
                connection.on('ReceiveTurnResult', (result: ProcessResult) => {
                    onTurnResultRef.current(result);
                });

                // バトル開始受信ハンドラ
                connection.on('BattleStarted', (state: BattleState) => {
                    if (onBattleStartedRef.current) {
                        onBattleStartedRef.current(state);
                    }
                });

                // バトル終了受信ハンドラ
                connection.on('BattleEnded', (endResult: EndResult) => {
                    if (onBattleEndedRef.current) {
                        onBattleEndedRef.current(endResult);
                    }
                });

                // エラー受信ハンドラ
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
    }, [battleId, token]); // コールバックを依存配列から削除

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
