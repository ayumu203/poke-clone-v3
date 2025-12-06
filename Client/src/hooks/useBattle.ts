import { useMutation,useQuery } from '@tanstack/react-query';

import { type BattleState } from '@/types/battle';

import { useApi } from './useApi';

export function useBattle(battleId?: string) {
    const { request } = useApi();

    // バトル状態の取得
    const { data: battleState, isLoading } = useQuery<BattleState>({
        queryKey: ['battle', battleId],
        queryFn: () => request(`/api/Battle/${battleId}`),
        enabled: !!battleId, // battleIdが存在する場合のみクエリを実行
        refetchInterval: false, // SignalRで更新されるため自動リフレッシュ不要
    });

    // CPUバトルの作成
    const createCpuBattle = useMutation({
        mutationFn: () => request('/api/Battle/cpu', { method: 'POST' }),
    });

    return {
        battleState,
        isLoading,
        createCpuBattle: createCpuBattle.mutate,
        isCreating: createCpuBattle.isPending,
    };
}
