import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query';

import { useApi } from './useApi';

interface Player {
    playerId: string;
    name: string;
    iconUrl?: string;
}

export function usePlayer() {
    const { request } = useApi<Player>();
    const queryClient = useQueryClient();

    // プレイヤー情報の取得
    const { data: player, isLoading, error } = useQuery({
        queryKey: ['player', 'me'],
        queryFn: () => request('/api/Player/me'),
        staleTime: 5 * 60 * 1000, // 5分間キャッシュ
    });

    // プレイヤー情報の更新
    const updatePlayer = useMutation({
        mutationFn: (data: { name: string; iconUrl?: string }) =>
            request('/api/Player/me', { method: 'POST', body: data }),
        onSuccess: () => {
            // キャッシュを無効化して再取得
            queryClient.invalidateQueries({ queryKey: ['player', 'me'] });
        },
    });

    return {
        player,
        isLoading,
        error,
        updatePlayer: updatePlayer.mutate,
        isUpdating: updatePlayer.isPending,
    };
}
