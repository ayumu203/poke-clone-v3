"use client";

import { useMutation, useQueryClient } from "@tanstack/react-query";

import type { Pokemon } from "@/types/pokemon";

import { useApi } from "./useApi";

interface GachaResponse {
  message: string;
  pokemon: Pokemon;
}

export function useGacha() {
  const { request } = useApi<GachaResponse>();
  const queryClient = useQueryClient();

  // ガチャを引く
  const pullGacha = useMutation({
    mutationFn: async () => {
      const result = await request("/api/Gacha/pull", { method: "POST" });
      if (!result) throw new Error("ガチャの実行に失敗しました");
      return result;
    },
    onSuccess: () => {
      // パーティキャッシュを無効化して再取得
      queryClient.invalidateQueries({ queryKey: ["party"] });
    },
  });

  return {
    pullGacha: pullGacha.mutate,
    pullGachaAsync: pullGacha.mutateAsync,
    isPulling: pullGacha.isPending,
    result: pullGacha.data,
    error: pullGacha.error,
    isSuccess: pullGacha.isSuccess,
    reset: pullGacha.reset,
  };
}
