import { create } from "zustand"
import type { EnemyResponse } from "../../arena-enemies/fetchEnemy"
import type { ActionType } from "@/shared/types/action"

export type Enemy = EnemyResponse

type EnemiesState = {
  enemies: Record<string, Enemy>
  setEnemies: (enemies: Enemy[]) => void
  getEnemy: (id?: string) => Enemy | undefined
  getSpriteAnimation: (enemyId?: string, actionType?: ActionType) => Enemy["actionAssets"][number]["spriteAnimation"] | undefined
  reset: () => void
}

export const useEnemiesStore = create<EnemiesState>((set, get) => ({
  enemies: {},
  setEnemies: (enemies) =>
    set({
      enemies: Object.fromEntries(
        enemies.map((e) => [e.id, { ...e }])
      ),
    }),
  getEnemy: (id) => {
    if (!id)
      return undefined
    return get().enemies[id]
  },
  getSpriteAnimation: (enemyId, actionType) => {
    if (!enemyId || !actionType) return undefined
    const enemy = get().enemies[enemyId]
    if (!enemy) return undefined

    const variants = enemy.actionAssets
      .filter(a => a.actionType === actionType);

    if (variants.length === 0) return undefined;

    // выбираем случайный
    const randomIndex = Math.floor(Math.random() * variants.length);
    return variants[randomIndex].spriteAnimation;
  },
  reset: () => set({ enemies: {} }),
}))
