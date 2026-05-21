import { create } from "zustand"

export const FloatingCombatTextKind = {
  Damage: "damage",
  Healing: "healing",
} as const

export type FloatingCombatTextKind =
  (typeof FloatingCombatTextKind)[keyof typeof FloatingCombatTextKind]

export interface FloatingCombatText {
  id: string
  unitId: string
  value: number
  kind: FloatingCombatTextKind
}

interface FloatingCombatTextStore {
  items: FloatingCombatText[]
  add: (unitId: string, value: number, kind: FloatingCombatTextKind) => void
  remove: (id: string) => void
  reset: () => void
}

export const useFloatingCombatTextStore = create<FloatingCombatTextStore>((set) => ({
  items: [],

  add: (unitId, value, kind) => {
    set((s) => ({
      items: [
        ...s.items,
        {
          id: crypto.randomUUID(),
          unitId,
          value,
          kind,
        },
      ],
    }))
  },

  remove: (id) => {
    set((s) => ({
      items: s.items.filter((x) => x.id !== id),
    }))
  },

  reset: () => {
    set({ items: [] })
  },
}))
