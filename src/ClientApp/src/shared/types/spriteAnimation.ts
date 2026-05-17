export interface SpriteAnimationDto {
  url: string
  frameCount: number
  animationSpeed: number
  scale: ScaleDto
}

export interface ScaleDto {
  x: number
  y: number
}
