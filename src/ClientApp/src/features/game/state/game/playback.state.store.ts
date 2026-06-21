import { create } from "zustand"

export const ANIMATION_SPEED_PERCENT_DEFAULT = 100
export const ANIMATION_SPEED_PERCENT_MIN = 25
export const ANIMATION_SPEED_PERCENT_MAX = 300
export const ANIMATION_SPEED_PERCENT_STEP = 25

const ANIMATION_SPEED_STORAGE_KEY = "arena_animation_speed_percent"

interface PlaybackState {
  animationSpeedPercent: number
  animationSpeedScale: number
  setAnimationSpeedPercent: (value: number) => void
  adjustAnimationSpeedPercent: (delta: number) => void
}

function clampAnimationSpeedPercent(value: number) {
  if (!Number.isFinite(value)) return ANIMATION_SPEED_PERCENT_DEFAULT

  return Math.min(
    ANIMATION_SPEED_PERCENT_MAX,
    Math.max(ANIMATION_SPEED_PERCENT_MIN, Math.round(value)),
  )
}

function readInitialAnimationSpeedPercent() {
  const savedValue = localStorage.getItem(ANIMATION_SPEED_STORAGE_KEY)
  if (!savedValue) return ANIMATION_SPEED_PERCENT_DEFAULT

  return clampAnimationSpeedPercent(Number(savedValue))
}

function createPlaybackState(animationSpeedPercent: number) {
  return {
    animationSpeedPercent,
    animationSpeedScale: animationSpeedPercent / 100,
  }
}

export const useArenaPlaybackStore = create<PlaybackState>((set, get) => ({
  ...createPlaybackState(readInitialAnimationSpeedPercent()),

  setAnimationSpeedPercent: (value) => {
    const nextValue = clampAnimationSpeedPercent(value)

    localStorage.setItem(ANIMATION_SPEED_STORAGE_KEY, String(nextValue))
    set(createPlaybackState(nextValue))
  },

  adjustAnimationSpeedPercent: (delta) => {
    get().setAnimationSpeedPercent(get().animationSpeedPercent + delta)
  },
}))

export function getAnimationSpeedScale() {
  return useArenaPlaybackStore.getState().animationSpeedScale
}
