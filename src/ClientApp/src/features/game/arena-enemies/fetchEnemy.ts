import type { StatDto } from "@/shared/types/stat"
import type { ActionAssetDto } from "@/shared/types/action"
import { apiFetch } from "@/shared/lib/apiFetch"
import type { AbilityDto } from "@/shared/types/ability"

export interface EnemyResponse {
  id: string
  name: string
  description?: string
  stats: StatDto[]
  actionAssets: ActionAssetDto[]
  abilities: AbilityDto[]
}

export async function fetchEnemy(enemyId: string): Promise<EnemyResponse> {
  return apiFetch<EnemyResponse>(`/enemies/${enemyId}`)
}
