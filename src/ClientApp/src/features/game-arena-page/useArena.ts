import { useQuery } from "@tanstack/react-query"
import { ApiException, apiFetch } from "@/shared/lib/apiFetch"
import { queryKeys } from "@/shared/lib/queryKeys"

export type Arena = {
  id: string
  name: string
  description?: string
  gridWidth: number
  gridHeight: number
  backgroundAsset?: string
}

export function useArena(arenaId: string | undefined) {
  return useQuery<Arena, ApiException>({
    queryKey: queryKeys.arenas.byId(arenaId),
    queryFn: () => apiFetch(`/arenas/${arenaId}`),
    enabled: !!arenaId,
  })
}
