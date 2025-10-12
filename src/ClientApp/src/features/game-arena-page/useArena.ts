import { useQuery } from "@tanstack/react-query"
import { apiFetch } from "@/lib/apiFetch"
import { queryKeys } from "@/lib/queryKeys"

export type Arena = {
  id: string
  name: string
  description?: string
  gridWidth: number
  gridHeight: number
  backgroundAsset?: string
}

export function useArena(arenaId: string | undefined) {
  return useQuery<Arena, Error>({
    queryKey: queryKeys.arenas.byId(arenaId),
    queryFn: () => apiFetch(`/arenas/${arenaId}`),
    enabled: !!arenaId,
  })
}
