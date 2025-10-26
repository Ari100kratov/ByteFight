import { useQuery } from "@tanstack/react-query"
import { ApiException, apiFetch } from "@/shared/lib/apiFetch"
import { queryKeys } from "@/shared/lib/queryKeys";
import type { StatDto } from "@/shared/types/stat";
import type { ActionAssetDto } from "@/shared/types/action";

export type EnemyResponse = {
  id: string
  name: string
  description?: string
  stats: StatDto[]
  actionAssets: ActionAssetDto[]
}

export function useEnemy(enemyId?: string) {
  return useQuery<EnemyResponse, ApiException>({
    queryKey: queryKeys.enemies.byId(enemyId),
    queryFn: () => apiFetch<EnemyResponse>(`/enemies/${enemyId}`),
    enabled: !!enemyId
  })
}
