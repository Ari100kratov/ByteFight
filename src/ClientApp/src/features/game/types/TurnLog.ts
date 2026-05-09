import type { AbilityEffectType, AbilityType } from "../../../shared/types/ability";
import type { FacingDirection, Position, StatSnapshot } from "./common";

/**
 * Тип записи журнала боя.
 */
export const GameActionLogEntryType = {
  Idle: 1,
  Walk: 2,
  AbilityUsed: 3,
  Death: 4,
} as const;

export type GameActionLogEntryType =
  typeof GameActionLogEntryType[keyof typeof GameActionLogEntryType];

export type TurnLog = {
  turnIndex: number;
  logs: GameActionLogEntry[];
};

export type BaseLogEntry = {
  id: string;
  entryType: GameActionLogEntryType;
  actorId: string;
  actorName: string;
  info?: string | null;
  turnIndex: number;
  createdAt: string;
};

export type GameActionLogEntry =
  | AbilityUsedLogEntry
  | WalkLogEntry
  | DeathLogEntry
  | IdleLogEntry;

/**
 * Использование способности (универсально для атак, лечения и т.д.)
 */
export type AbilityUsedLogEntry = BaseLogEntry & {
  entryType: typeof GameActionLogEntryType.AbilityUsed;

  abilityType: AbilityType;
  effectType: AbilityEffectType;
  abilityName?: string | null;

  targetId: string;
  targetName: string;
  value: number;
  facingDirection: FacingDirection;
  targetHp: StatSnapshot;
};

/**
 * Перемещение
 */
export type WalkLogEntry = BaseLogEntry & {
  entryType: typeof GameActionLogEntryType.Walk;

  facingDirection: FacingDirection;
  to: Position;
};

/**
 * Смерть
 */
export type DeathLogEntry = BaseLogEntry & {
  entryType: typeof GameActionLogEntryType.Death;
};

/**
 * Простой пропуск хода
 */
export type IdleLogEntry = BaseLogEntry & {
  entryType: typeof GameActionLogEntryType.Idle;
};

/**
 * Type guards
 */
export const isAbilityUsed = (
  e: GameActionLogEntry
): e is AbilityUsedLogEntry =>
  e.entryType === GameActionLogEntryType.AbilityUsed;

export const isWalk = (e: GameActionLogEntry): e is WalkLogEntry =>
  e.entryType === GameActionLogEntryType.Walk;

export const isDeath = (e: GameActionLogEntry): e is DeathLogEntry =>
  e.entryType === GameActionLogEntryType.Death;

export const isIdle = (e: GameActionLogEntry): e is IdleLogEntry =>
  e.entryType === GameActionLogEntryType.Idle;