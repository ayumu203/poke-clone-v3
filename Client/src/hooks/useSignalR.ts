import type * as signalR from '@microsoft/signalr';
import { useEffect, useState } from 'react';

import { signalRService } from '@/lib/signalr/connection';
import { type PlayerAction } from '@/types/action';
import { type BattleState, type ProcessResult } from '@/types/battle';

export const useSignalR = (
    battleId: string,
    token: string,
    onTurnResult: (result: ProcessResult) => void,
    onBattleStarted?: (state: BattleState) => void
) => {
    const [connection, setConnection] = useState<signalR.HubConnection | null>(null);
    const [isConnected, setIsConnected] = useState(false);
    const [error, setError] = useState<string | null>(null);

    useEffect(() => {
        const setupConnection = async () => {
            try {
                const conn = await signalRService.connect(token);

                // ターン結果受信ハンドラ
                conn.on('ReceiveTurnResult', (result: ProcessResult) => {
                    onTurnResult(result);
                });

                // バトル開始受信ハンドラ
                conn.on('BattleStarted', (state: BattleState) => {
                    if (onBattleStarted) {
                        onBattleStarted(state);
                    }
                });

                // バトル終了受信ハンドラ
                conn.on('BattleEnded', (_endResult: unknown) => {
                    // バトル終了処理
                });

                // エラー受信ハンドラ
                conn.on('Error', (errorMessage: string) => {
                    console.error('SignalR Error:', errorMessage);
                    setError(errorMessage);
                });

                // バトルルームに参加
                await conn.invoke('JoinBattle', battleId);

                setConnection(conn);
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
    }, [battleId, token, onTurnResult, onBattleStarted]);

    const submitAction = async (action: PlayerAction) => {
        if (!connection) {
            throw new Error('SignalR not connected');
        }

        try {
            await connection.invoke('SubmitAction', battleId, action);
        } catch (err) {
            console.error('Failed to submit action:', err);
            throw err;
        }
    };

    return { connection, isConnected, error, submitAction };
};
