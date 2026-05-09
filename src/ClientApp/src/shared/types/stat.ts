export const StatType = {
  Health: 1,
  Mana: 2,
  MoveRange: 3,
} as const

export type StatType = typeof StatType[keyof typeof StatType]

export type StatDto = {
  statType: StatType
  value: number
}

export function getStatName(type: number) {
  switch (type) {
    case StatType.Health: return "Здоровье"
    case StatType.Mana: return "Мана"
    case StatType.MoveRange: return "Перемещение"
    default: return "?"
  }
}