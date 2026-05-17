export const ArenaItemType = {
  HealingPotion: 1,
} as const

export type ArenaItemType = (typeof ArenaItemType)[keyof typeof ArenaItemType]
