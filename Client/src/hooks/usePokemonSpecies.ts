import { useQuery, useQueries } from '@tanstack/react-query';

import type { PokemonSpecies } from '@/types/pokemonSpecies';

import { useApi } from './useApi';

/**
 * ポケモン種族情報を取得するフック
 * pokemonSpeciesIdをキーにしてキャッシュ
 */
export function usePokemonSpecies(pokemonSpeciesId: number) {
    const { request } = useApi<PokemonSpecies>();

    const { data: species, isLoading, error } = useQuery({
        queryKey: ['pokemonSpecies', pokemonSpeciesId],
        queryFn: async () => {
            const result = await request(`/api/Pokemon/${pokemonSpeciesId}`);
            if (!result) throw new Error('Failed to fetch pokemon species');
            return result;
        },
        staleTime: 5 * 60 * 1000, // 5分間キャッシュ
        enabled: !!pokemonSpeciesId,
    });

    return {
        species,
        isLoading,
        error,
    };
}

/**
 * 複数のポケモン種族情報を一括で取得するフック
 * パーティ画面などで複数のポケモンを表示する際に使用
 */
export function usePokemonSpeciesBatch(pokemonSpeciesIds: number[]) {
    const { request } = useApi<PokemonSpecies>();

    const results = useQueries({
        queries: pokemonSpeciesIds.map(id => ({
            queryKey: ['pokemonSpecies', id],
            queryFn: async () => {
                const result = await request(`/api/Pokemon/${id}`);
                if (!result) throw new Error('Failed to fetch pokemon species');
                return result;
            },
            staleTime: 5 * 60 * 1000,
            enabled: !!id,
        })),
    });

    const isLoading = results.some(r => r.isLoading);
    const species = results.map(r => r.data);

    return {
        species,
        isLoading,
    };
}

