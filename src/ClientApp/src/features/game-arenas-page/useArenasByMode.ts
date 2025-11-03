import { useQuery } from "@tanstack/react-query"
import { ApiException, apiFetch } from "@/shared/lib/apiFetch"
import { queryKeys } from "@/shared/lib/queryKeys"

export type ArenaResponse = {
  id: string
  name: string
  description?: string
  gridWidth: number
  gridHeight: number
}

export function useArenasByMode(mode: string | undefined) {
  return useQuery<ArenaResponse[], ApiException>({
    queryKey: queryKeys.arenas.byMode(mode),
    queryFn: () => apiFetch(`/arenas?mode=${mode}`),
    enabled: !!mode
  })
}