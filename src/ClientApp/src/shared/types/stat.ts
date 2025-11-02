export const StatType = {
  Health: 1,
  Mana: 2,
  Attack: 3,
  AttackRange: 4,
  MoveRange: 5,
} as const;

export type StatType = (typeof StatType)[keyof typeof StatType];

export type StatDto = {
  statType: StatType;
  value: number;
}

export function getStatName(type: number) {
  switch (type) {
    case StatType.Health: return "Здоровье"
    case StatType.Mana: return "Мана"
    case StatType.Attack: return "Атака"
    case StatType.AttackRange: return "Дальность атаки"
    case StatType.MoveRange: return "Скорость перемещения"
    default: return "?"
  }
}
