import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query';

import { useApi } from './useApi';

interface Move {
    moveId: number;
    name: string;
    type: string;
    damageClass: string;
    power: number;
    accuracy: number;
    pp: number;
}

interface Pokemon {
    pokemonId: string;
    pokemonSpeciesId: number;
    level: number;
    exp: number;
    moves: Move[];
}

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
        staleTime: 1 * 60 * 1000, // 1分間キャッシュ
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
