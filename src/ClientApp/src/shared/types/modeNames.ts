import { GameModeType } from "@/features/game/types/GameSession"

export const ModeSlugs = {
  [GameModeType.Training]: "training",
  [GameModeType.PvE]: "pve",
  [GameModeType.PvP]: "pvp",
} as const

export const ModeNamesBySlug = {
  training: "Тренировка",
  pve: "PvE",
  pvp: "PvP",
} as const

export type GameModeSlug = keyof typeof ModeNamesBySlug

export function formatModeSlugByType(mode: GameModeType): GameModeSlug {
  return ModeSlugs[mode] ?? "training"
}

export function formatModeNameByType(mode: GameModeType): string {
  return ModeNamesBySlug[formatModeSlugByType(mode)]
}

export function formatModeNameByString(mode: string | undefined): string {
  return ModeNamesBySlug[mode as GameModeSlug] ?? mode
}