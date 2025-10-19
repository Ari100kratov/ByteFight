import { useQuery } from "@tanstack/react-query"
import { ApiException, apiFetch } from "@/lib/apiFetch"
import { queryKeys } from "@/lib/queryKeys";

export const StatType = {
  Health: 1,
  Mana: 2,
  Attack: 3,
  Defense: 4,
  Movement: 5,
} as const;

export type StatType = (typeof StatType)[keyof typeof StatType];

export type EnemyStatDto = {
  statType: StatType
  value: number
}

export const ActionType = {
  Idle: 1,
  Walk: 2,
  Attack: 3,
  Hit: 4,
  Death: 5,
} as const;

export type ActionType = (typeof ActionType)[keyof typeof ActionType];

export type EnemyAssetDto = {
  actionType: ActionType
  url: string
}

export type EnemyResponse = {
  id: string
  name: string
  description?: string
  stats: EnemyStatDto[]
  assets: EnemyAssetDto[]
}

export function useEnemy(enemyId?: string) {
  return useQuery<EnemyResponse, ApiException>({
    queryKey: queryKeys.enemies.byId(enemyId),
    queryFn: () => apiFetch<EnemyResponse>(`/enemies/${enemyId}`),
    enabled: !!enemyId
  })
}
