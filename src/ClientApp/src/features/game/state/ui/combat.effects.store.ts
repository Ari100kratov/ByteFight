import type { Position } from "../../types/common"
import { create } from "zustand"

export const CombatVisualEffectKind = {
  DamageImpact: "damage-impact",
  HealingPulse: "healing-pulse",
  ItemPickup: "item-pickup",
} as const

export type CombatVisualEffectKind =
  (typeof CombatVisualEffectKind)[keyof typeof CombatVisualEffectKind]

export type CombatVisualEffectAnchor =
  | {
      type: "unit"
      unitId: string
    }
  | {
      type: "cell"
      position: Position
    }

export interface CombatVisualEffect {
  id: string
  kind: CombatVisualEffectKind
  anchor: CombatVisualEffectAnchor
  seed: string
}

interface CombatEffectsState {
  effects: CombatVisualEffect[]
  addUnitEffect: (unitId: string, kind: CombatVisualEffectKind, seed?: string) => void
  addCellEffect: (position: Position, kind: CombatVisualEffectKind, seed?: string) => void
  remove: (id: string) => void
  reset: () => void
}

export const useCombatEffectsStore = create<CombatEffectsState>((set) => ({
  effects: [],

  addUnitEffect: (unitId, kind, seed) => {
    set((state) => ({
      effects: [
        ...state.effects,
        {
          id: crypto.randomUUID(),
          kind,
          anchor: { type: "unit", unitId },
          seed: seed ?? `${unitId}:${kind}`,
        },
      ],
    }))
  },

  addCellEffect: (position, kind, seed) => {
    set((state) => ({
      effects: [
        ...state.effects,
        {
          id: crypto.randomUUID(),
          kind,
          anchor: { type: "cell", position },
          seed: seed ?? `${String(position.x)}:${String(position.y)}:${kind}`,
        },
      ],
    }))
  },

  remove: (id) => {
    set((state) => ({
      effects: state.effects.filter((effect) => effect.id !== id),
    }))
  },

  reset: () => {
    set({ effects: [] })
  },
}))
