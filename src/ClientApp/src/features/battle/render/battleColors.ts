/**
 * Древнеславянская палитра боя: тёмная хвоя, золото, окись железа.
 */

export const COLORS = {
  canvasBg: 0x11180f,

  meadowFill: 0x43603a,
  forestFill: 0x2c4a2a,
  swampFill: 0x555a37,
  waterFill: 0x274a5c,
  rockFill: 0x585850,

  gridStroke: 0x20301e,
  gridStrokeHover: 0x9c7b3a,

  reachableFill: 0xd9c07a,
  reachableAlpha: 0.28,

  pathFill: 0xe8d9a0,
  pathAlpha: 0.5,

  targetFill: 0xc2543f,
  targetAlpha: 0.4,

  playerPlate: 0xd9a441,
  playerPlateDark: 0x8a6420,
  enemyPlate: 0xa34040,
  enemyPlateDark: 0x5f2626,

  hpBack: 0x201510,
  hpFill: 0x7fae4c,
  hpLow: 0xc2543f,
  manaFill: 0x4c7fae,

  activeRing: 0xe8d9a0,

  damageText: '#ff8a6a',
  healText: '#9fe06a',
  statusText: '#e8d9a0',
  infoText: '#c9c9b0',
} as const

export const TERRAIN_FILL: Record<number, number> = {
  1: COLORS.meadowFill,
  2: COLORS.forestFill,
  3: COLORS.swampFill,
  4: COLORS.waterFill,
  5: COLORS.rockFill,
}
