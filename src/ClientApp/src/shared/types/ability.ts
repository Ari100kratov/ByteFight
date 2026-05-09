import type { ActionAssetDto } from "@/shared/types/action"

export const AbilityType = {
  BasicMeleeAttack: 1,
  BasicRangedAttack: 2,
  Healing: 3
} as const

export type AbilityType = typeof AbilityType[keyof typeof AbilityType]

export const AbilityEffectType = {
  Damage: 1,
  Healing: 2,
  Shield: 3,
  Buff: 4,
  Debuff: 5,
} as const

export type AbilityEffectType =
  typeof AbilityEffectType[keyof typeof AbilityEffectType]

export const AbilityTargetType = {
  Enemy: 1,
  Self: 2,
  Ally: 3,
  Area: 4,
} as const

export type AbilityTargetType =
  typeof AbilityTargetType[keyof typeof AbilityTargetType]

export const AbilityStatType = {
  Range: 1,
  Damage: 2,
  Healing: 3,
  ManaCost: 4,
  AreaRadius: 5,
} as const

export type AbilityStatType =
  typeof AbilityStatType[keyof typeof AbilityStatType]

export type AbilityStatDto = {
  statType: AbilityStatType
  value: number
}

export type AbilityDto = {
  type: AbilityType
  effectType: AbilityEffectType
  targetType: AbilityTargetType
  name: string
  description?: string | null
  priority: number
  stats: AbilityStatDto[]
  actionAssets: ActionAssetDto[]
}