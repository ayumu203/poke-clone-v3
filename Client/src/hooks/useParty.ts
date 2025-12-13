import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query';

import type { Pokemon } from '@/types/pokemon';

import { useApi } from './useApi';

export function useParty() {
    const { request } = useApi<Pokemon[]>();
    const queryClient = useQueryClient();

    // パーティ一覧の取得
    const { data: party, isLoading, error } = useQuery({
        queryKey: ['party'],
        queryFn: async () => {
            const result = await request('/api/Party');
            if (!result) throw new Error('Failed to fetch party');
            return result;
        },
        staleTime: 0, // キャッシュせず毎回取得
    });

    // ポケモンを逃がす
    const releasePokemon = useMutation({
        mutationFn: (pokemonId: string) =>
            request(`/api/Party/${pokemonId}`, { method: 'DELETE' }),
        onSuccess: () => {
            queryClient.invalidateQueries({ queryKey: ['party'] });
        },
    });

    return {
        party,
        isLoading,
        error,
        releasePokemon: releasePokemon.mutate,
        isReleasing: releasePokemon.isPending,
    };
}
