import { useQuery } from "@tanstack/react-query"
import { apiFetch } from "@/lib/apiFetch"
import { queryKeys } from "@/lib/queryKeys"

export type Arena = {
  id: string
  name: string
  description?: string
  gridWidth: number
  gridHeight: number
}

export function useArenasByMode(mode: string | undefined) {
  return useQuery<Arena[], Error>({
    queryKey: queryKeys.arenas.byMode(mode),
    queryFn: () => apiFetch(`/arenas?mode=${mode}`),
    enabled: !!mode
  })
}