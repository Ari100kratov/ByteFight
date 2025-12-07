import { ActionType } from "@/shared/types/action";
import type { FacingDirection, Position, StatSnapshot } from "./common";

export type TurnLog = {
  turnIndex: number;
  logs: RuntimeLogEntry[];
};

export type RuntimeLogEntry =
  | AttackLogEntry
  | WalkLogEntry
  | DeathLogEntry
  | IdleLogEntry;

// Attack
export type AttackLogEntry = {
  type: typeof ActionType.Attack;
  actorId: string;
  targetId: string;
  damage: number;
  facingDirection: FacingDirection;
  targetHp: StatSnapshot;
};

// Walk
export type WalkLogEntry = {
  type: typeof ActionType.Walk;
  actorId: string;
  facingDirection: FacingDirection;
  to: Position;
};

// Death
export type DeathLogEntry = {
  type: typeof ActionType.Dead;
  actorId: string;
};

// Idle
export type IdleLogEntry = {
  type: typeof ActionType.Idle;
  actorId: string;
};

export const isAttack = (e: RuntimeLogEntry): e is AttackLogEntry =>
  e.type === ActionType.Attack;

export const isWalk = (e: RuntimeLogEntry): e is WalkLogEntry =>
  e.type === ActionType.Walk;

export const isDeath = (e: RuntimeLogEntry): e is DeathLogEntry =>
  e.type === ActionType.Dead;

export const isIdle = (e: RuntimeLogEntry): e is IdleLogEntry =>
  e.type === ActionType.Idle;
