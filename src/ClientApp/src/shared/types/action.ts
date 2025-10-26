import type { SpriteAnimationDto } from "./spriteAnimation";

export const ActionType = {
  Idle: 1,
  Walk: 2,
  Run: 3,
  Attack: 4,
  Run_Attack: 5,
  Jump: 6,
  Hurt: 7,
  Dead: 8,
} as const;

export type ActionType = (typeof ActionType)[keyof typeof ActionType];

export type ActionAssetDto = {
  actionType: ActionType;
  variant: number;
  spriteAnimation: SpriteAnimationDto;
}

