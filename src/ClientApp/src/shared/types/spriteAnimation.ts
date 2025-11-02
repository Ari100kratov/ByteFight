export type SpriteAnimationDto = {
  url: string;
  frameCount: number;
  animationSpeed: number;
  scale: ScaleDto
}

export type ScaleDto = {
  x: number;
  y: number;
}