import { useMutation, useQuery } from '@tanstack/react-query';

import { useApi } from './useApi';

interface StarterOption {
    pokemonSpeciesId: number;
    name: string;
    frontImage: string;
    backImage: string;
    type1: string;
    type2?: string;
    baseHp: number;
    baseAttack: number;
    baseDefence: number;
    baseSpecialAttack: number;
    baseSpecialDefence: number;
    baseSpeed: number;
}

export function useStarter() {
    const { request } = useApi<StarterOption[]>();

    // スターター選択肢の取得
    const { data: options, isLoading } = useQuery<StarterOption[]>({
        queryKey: ['starter', 'options'],
        queryFn: async () => {
            const result = await request('/api/Starter/options');
            if (!result) throw new Error('Failed to fetch starter options');
            return result;
        },
    });

    // スターターを選択
    const selectStarter = useMutation({
        mutationFn: (pokemonSpeciesId: number) =>
            request('/api/Starter/select', {
                method: 'POST',
                body: { pokemonSpeciesId },
            }),
    });

    return {
        options,
        isLoading,
        selectStarter: selectStarter.mutate,
        isSelecting: selectStarter.isPending,
    };
}
