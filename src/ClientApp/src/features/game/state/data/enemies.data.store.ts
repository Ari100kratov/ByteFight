import { create } from "zustand"
import type { EnemyResponse } from "../../arena-enemies/fetchEnemy"
import type { ActionType } from "@/shared/types/action"
import type { AbilityType } from "@/shared/types/ability"

export type Enemy = EnemyResponse

interface EnemiesState {
  enemies: Partial<Record<string, Enemy>>
  setEnemies: (enemies: Enemy[]) => void
  getEnemy: (id?: string) => Enemy | undefined
  getSpriteAnimation: (
    enemyId?: string,
    actionType?: ActionType,
  ) => Enemy["actionAssets"][number]["spriteAnimation"] | undefined
  getAbilitySpriteAnimation: (
    enemyId?: string,
    abilityType?: AbilityType,
    actionType?: ActionType,
  ) => Enemy["abilities"][number]["actionAssets"][number]["spriteAnimation"] | undefined
  reset: () => void
}

export const useEnemiesStore = create<EnemiesState>((set, get) => ({
  enemies: {},

  setEnemies: (enemies) => {
    set({
      enemies: Object.fromEntries(enemies.map((e) => [e.id, { ...e }])),
    })
  },

  getEnemy: (id) => {
    if (!id) return undefined
    return get().enemies[id]
  },

  getSpriteAnimation: (enemyId, actionType) => {
    if (!enemyId || !actionType) return undefined

    const enemy = get().enemies[enemyId]
    if (!enemy) return undefined

    const variants = enemy.actionAssets.filter((a) => a.actionType === actionType)

    if (variants.length === 0) return undefined

    const randomIndex = Math.floor(Math.random() * variants.length)
    return variants.at(randomIndex)?.spriteAnimation
  },

  getAbilitySpriteAnimation: (enemyId, abilityType, actionType) => {
    if (!enemyId || !abilityType || !actionType) return undefined

    const enemy = get().enemies[enemyId]
    if (!enemy) return undefined

    const ability = enemy.abilities.find((a) => a.type === abilityType)

    if (!ability) return undefined

    const variants = ability.actionAssets.filter((a) => a.actionType === actionType)

    if (variants.length === 0) return undefined

    const randomIndex = Math.floor(Math.random() * variants.length)
    return variants.at(randomIndex)?.spriteAnimation
  },

  reset: () => {
    set({ enemies: {} })
  },
}))
