import { useMutation, useQuery } from '@tanstack/react-query';

import type { StarterOption } from '@/types/starter';

import { useApi } from './useApi';

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
    const selectStarterMutation = useMutation({
        mutationFn: (pokemonSpeciesId: number) =>
            request('/api/Starter/select', {
                method: 'POST',
                body: { pokemonSpeciesId },
            }),
    });

    return {
        options,
        isLoading,
        selectStarter: selectStarterMutation.mutate,
        selectStarterAsync: selectStarterMutation.mutateAsync,
        isSelecting: selectStarterMutation.isPending,
    };
}
