import { useCallback, useState } from 'react';

import type { UseApiOptions } from '@/types/api';

export function useApi<T = unknown>() {
    const [data, setData] = useState<T | null>(null);
    const [error, setError] = useState<string | null>(null);
    const [loading, setLoading] = useState(false);

    const request = useCallback(async (
        url: string,
        options: UseApiOptions = {}
    ): Promise<T | null> => {
        setLoading(true);
        setError(null);

        try {
            const apiUrl = process.env.API_URL || 'http://localhost:5000';

            const response = await fetch(`${apiUrl}${url}`, {
                method: options.method || 'GET',
                headers: {
                    'Content-Type': 'application/json',
                    ...options.headers,
                },
                credentials: 'include', // Send cookies with request
                body: options.body ? JSON.stringify(options.body) : undefined,
            });

            if (!response.ok) {
                throw new Error(`HTTP ${response.status}: ${response.statusText}`);
            }

            const result = await response.json();
            setData(result);
            return result;
        } catch (err) {
            const errorMessage = err instanceof Error ? err.message : 'Unknown error';
            setError(errorMessage);
            console.error('API request failed:', errorMessage);
            return null;
        } finally {
            setLoading(false);
        }
    }, []);

    return { data, error, loading, request };
}
