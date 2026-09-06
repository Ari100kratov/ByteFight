/**
 * Типы состояния боя — зеркало серверных BattleStateDto и записей журнала.
 */

export const FacingDirection = {
  Right: 1,
  DownRight: 2,
  Down: 3,
  DownLeft: 4,
  Left: 5,
  UpLeft: 6,
  Up: 7,
  UpRight: 8,
} as const

export type FacingDirectionValue = (typeof FacingDirection)[keyof typeof FacingDirection]

export interface HexDto {
  x: number
  y: number
}

export const TerrainType = {
  Meadow: 1,
  Forest: 2,
  Swamp: 3,
  Water: 4,
  Rock: 5,
} as const

export type TerrainTypeValue = (typeof TerrainType)[keyof typeof TerrainType]

export const TERRAIN_NAMES: Record<number, string> = {
  1: 'Луговина',
  2: 'Чаща',
  3: 'Топь',
  4: 'Вода',
  5: 'Камень',
}

export const TERRAIN_HINTS: Record<number, string> = {
  1: 'Обычный гекс',
  2: 'Вход 2 очка, урон по юниту −20%',
  3: 'Вход 2 очка, урон по юниту +25%',
  4: 'Непроходима',
  5: 'Непроходим, блокирует взгляд',
}

export interface BattleStatusDto {
  type: number
  turns: number
  magnitude: number
}

export interface BattleAbilityDto {
  type: number
  name: string
  effectType: number
  targetType: number
  shape: number
  range: number
  areaRadius: number
  damage: number
  healing: number
  manaCost: number
  cooldown: number
  actionCost: number
  dashesToTarget: boolean
  statuses: { type: number; duration: number; magnitude: number }[]
}

export interface BattleUnitDto {
  id: string
  name: string
  isPlayerSide: boolean
  isDead: boolean
  position: HexDto
  facing: number
  health: number
  maxHealth: number
  mana: number
  maxMana: number
  movePoints: number
  actions: number
  initiative: number
  statuses: BattleStatusDto[]
  cooldowns: Record<string, number>
  abilities: BattleAbilityDto[]
}

export interface BattleArenaDto {
  width: number
  height: number
  blocked: HexDto[]
  terrain: { x: number; y: number; terrain: number }[]
}

export interface BattleStateDto {
  round: number
  activeUnitId: string | null
  turnOrder: string[]
  units: BattleUnitDto[]
  arena: BattleArenaDto
}

// ===== Записи журнала боя =====

export interface BaseLogEntry {
  id: string
  actorId: string
  actorName: string
  info: string | null
  turnIndex: number
  createdAt: string
}

export interface IdleLogEntryDto extends BaseLogEntry {
  entryType: 'IdleLogEntryDto'
}

export interface WalkLogEntryDto extends BaseLogEntry {
  entryType: 'WalkLogEntryDto'
  facingDirection: number
  to: HexDto
  path: HexDto[] | null
}

export interface AbilityUsedLogEntryDto extends BaseLogEntry {
  entryType: 'AbilityUsedLogEntryDto'
  abilityType: number
  effectType: number
  abilityName: string | null
  targetId: string
  targetName: string
  value: number
  facingDirection: number
  targetHp: { current: number; max: number }
}

export interface DeathLogEntryDto extends BaseLogEntry {
  entryType: 'DeathLogEntryDto'
}

export interface ItemPickedUpLogEntryDto extends BaseLogEntry {
  entryType: 'ItemPickedUpLogEntryDto'
  itemName: string
  position: HexDto
  value: number
  actorHp: { current: number; max: number }
}

export interface StatusAppliedLogEntryDto extends BaseLogEntry {
  entryType: 'StatusAppliedLogEntryDto'
  targetId: string
  targetName: string
  statusType: number
  duration: number
  magnitude: number
  targetHp: { current: number; max: number } | null
}

export interface RoundStartedLogEntryDto extends BaseLogEntry {
  entryType: 'RoundStartedLogEntryDto'
  roundNumber: number
}

export type BattleLogEntry =
  | IdleLogEntryDto
  | WalkLogEntryDto
  | AbilityUsedLogEntryDto
  | DeathLogEntryDto
  | ItemPickedUpLogEntryDto
  | StatusAppliedLogEntryDto
  | RoundStartedLogEntryDto

export interface TurnLogDto {
  turnIndex: number
  logs: BattleLogEntry[]
}

// ===== Названия статусов для UI =====

const STATUS_META = new Map<number, { name: string; kind: 'good' | 'bad' }>([
  [1, { name: 'Горение', kind: 'bad' }],
  [2, { name: 'Яд', kind: 'bad' }],
  [3, { name: 'Регенерация', kind: 'good' }],
  [4, { name: 'Щит', kind: 'good' }],
  [5, { name: 'Замедление', kind: 'bad' }],
  [6, { name: 'Оковы корней', kind: 'bad' }],
  [7, { name: 'Оглушение', kind: 'bad' }],
  [8, { name: 'Немощь', kind: 'bad' }],
  [9, { name: 'Мощь', kind: 'good' }],
  [10, { name: 'Оберег', kind: 'good' }],
])

/** Название и характер статуса; неизвестные показываются как нейтральные. */
export function statusMeta(type: number): { name: string; kind: 'good' | 'bad' } {
  return STATUS_META.get(type) ?? { name: 'Влияние', kind: 'bad' }
}

export const ABILITY_EFFECT_NAMES: Record<number, string> = {
  1: 'Урон',
  2: 'Лечение',
  3: 'Щит',
  4: 'Усиление',
  5: 'Ослабление',
}

export const ABILITY_SHAPE_NAMES: Record<number, string> = {
  1: 'Одиночная цель',
  2: 'Радиус',
  3: 'Линия',
  4: 'Клин',
}
