export const StatType = {
  Health: 1,
  Mana: 2,
  Attack: 3,
  MoveRange: 4,
} as const;

export type StatType = (typeof StatType)[keyof typeof StatType];

export type StatDto = {
  statType: StatType;
  value: number;
}
