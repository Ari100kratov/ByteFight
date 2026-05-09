import { AbilityEffectType } from "@/shared/types/ability"
import { create } from "zustand"

export type FloatingCombatText = {
  id: string
  unitId: string
  value: number
  effectType: AbilityEffectType
}

type FloatingCombatTextStore = {
  items: FloatingCombatText[]
  add: (
    unitId: string,
    value: number,
    effectType: AbilityEffectType
  ) => void
  remove: (id: string) => void
  reset: () => void
}

export const useFloatingCombatTextStore = create<FloatingCombatTextStore>(
  set => ({
    items: [],

    add: (unitId, value, effectType) =>
      set(s => ({
        items: [
          ...s.items,
          {
            id: crypto.randomUUID(),
            unitId,
            value,
            effectType,
          },
        ],
      })),

    remove: id =>
      set(s => ({
        items: s.items.filter(x => x.id !== id),
      })),

    reset: () => set({ items: [] }),
  })
)