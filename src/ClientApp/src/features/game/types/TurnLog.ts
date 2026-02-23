import { ActionType } from "@/shared/types/action";
import type { FacingDirection, Position, StatSnapshot } from "./common";

export type TurnLog = {
  turnIndex: number;
  logs: GameActionLogEntry[];
};

export type BaseLogEntry = {
  id: string;
  actionType: ActionType;
  actorId: string;
  info?: string | null;
  turnIndex: number;
  createdAt: string;
};

export type GameActionLogEntry =
  | AttackLogEntry
  | WalkLogEntry
  | DeathLogEntry
  | IdleLogEntry;

// Attack
export type AttackLogEntry = BaseLogEntry & {
  actionType: typeof ActionType.Attack;

  targetId: string;
  damage: number;
  facingDirection: FacingDirection;
  targetHp: StatSnapshot;
};

// Walk
export type WalkLogEntry = BaseLogEntry & {
  actionType: typeof ActionType.Walk;

  facingDirection: FacingDirection;
  to: Position;
};

// Death
export type DeathLogEntry = BaseLogEntry & {
  actionType: typeof ActionType.Dead;
};

// Idle
export type IdleLogEntry = BaseLogEntry & {
  actionType: typeof ActionType.Idle;
};

export const isAttack = (e: GameActionLogEntry): e is AttackLogEntry =>
  e.actionType === ActionType.Attack;

export const isWalk = (e: GameActionLogEntry): e is WalkLogEntry =>
  e.actionType === ActionType.Walk;

export const isDeath = (e: GameActionLogEntry): e is DeathLogEntry =>
  e.actionType === ActionType.Dead;

export const isIdle = (e: GameActionLogEntry): e is IdleLogEntry =>
  e.actionType === ActionType.Idle;
