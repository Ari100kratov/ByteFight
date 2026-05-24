import { create } from "zustand"

export type ZoomDirection = "in" | "out"

interface CameraBounds {
  viewportWidth: number
  viewportHeight: number
  worldWidth: number
  worldHeight: number
}

interface Point {
  x: number
  y: number
}

interface CameraState {
  x: number
  y: number
  scale: number
  isDragging: boolean
  suppressClickUntil: number
  bounds?: CameraBounds
  setBounds: (bounds: CameraBounds) => void
  setDragging: (isDragging: boolean) => void
  panBy: (delta: Point) => void
  zoomAt: (point: Point, direction: ZoomDirection) => void
  zoomAtCenter: (direction: ZoomDirection) => void
  reset: () => void
  suppressClick: () => void
  isClickSuppressed: () => boolean
  worldToScreen: (point: Point) => Point
  screenToWorld: (point: Point) => Point
}

const ZOOM_LEVELS = [0.75, 1, 1.25, 1.5, 2] as const
const DEFAULT_SCALE = 1
const CLICK_SUPPRESSION_MS = 160
export const CAMERA_ZOOM_ENABLED = import.meta.env.VITE_ARENA_CAMERA_ZOOM === "true"

function clamp(value: number, min: number, max: number) {
  return Math.min(max, Math.max(min, value))
}

function clampCamera(x: number, y: number, scale: number, bounds: CameraBounds) {
  const scaledWorldWidth = bounds.worldWidth * scale
  const scaledWorldHeight = bounds.worldHeight * scale

  const nextX =
    scaledWorldWidth <= bounds.viewportWidth
      ? Math.round((bounds.viewportWidth - scaledWorldWidth) / 2)
      : Math.round(clamp(x, bounds.viewportWidth - scaledWorldWidth, 0))

  const nextY =
    scaledWorldHeight <= bounds.viewportHeight
      ? Math.round((bounds.viewportHeight - scaledWorldHeight) / 2)
      : Math.round(clamp(y, bounds.viewportHeight - scaledWorldHeight, 0))

  return { x: nextX, y: nextY, scale }
}

function getCenteredCamera(scale: number, bounds: CameraBounds) {
  return clampCamera(
    (bounds.viewportWidth - bounds.worldWidth * scale) / 2,
    (bounds.viewportHeight - bounds.worldHeight * scale) / 2,
    scale,
    bounds,
  )
}

function getNextZoom(scale: number, direction: ZoomDirection) {
  if (direction === "in") {
    return ZOOM_LEVELS.find((level) => level > scale + 0.001) ?? ZOOM_LEVELS.at(-1) ?? scale
  }

  return [...ZOOM_LEVELS].reverse().find((level) => level < scale - 0.001) ?? ZOOM_LEVELS[0]
}

function isDifferentWorld(a: CameraBounds | undefined, b: CameraBounds) {
  return a?.worldWidth !== b.worldWidth || a.worldHeight !== b.worldHeight
}

export const useCameraStore = create<CameraState>((set, get) => ({
  x: 0,
  y: 0,
  scale: DEFAULT_SCALE,
  isDragging: false,
  suppressClickUntil: 0,
  bounds: undefined,

  setBounds: (bounds) => {
    set((state) => {
      if (isDifferentWorld(state.bounds, bounds)) {
        return {
          ...getCenteredCamera(DEFAULT_SCALE, bounds),
          bounds,
        }
      }

      return {
        ...clampCamera(state.x, state.y, state.scale, bounds),
        bounds,
      }
    })
  },

  setDragging: (isDragging) => {
    set({ isDragging })
  },

  panBy: (delta) => {
    const state = get()
    if (!state.bounds) return

    set(clampCamera(state.x + delta.x, state.y + delta.y, state.scale, state.bounds))
  },

  zoomAt: (point, direction) => {
    const state = get()
    if (!state.bounds) return

    const nextScale = getNextZoom(state.scale, direction)
    if (nextScale === state.scale) return

    const worldPoint = {
      x: (point.x - state.x) / state.scale,
      y: (point.y - state.y) / state.scale,
    }

    set(
      clampCamera(
        point.x - worldPoint.x * nextScale,
        point.y - worldPoint.y * nextScale,
        nextScale,
        state.bounds,
      ),
    )
  },

  zoomAtCenter: (direction) => {
    const state = get()
    if (!state.bounds) return

    get().zoomAt(
      {
        x: state.bounds.viewportWidth / 2,
        y: state.bounds.viewportHeight / 2,
      },
      direction,
    )
  },

  reset: () => {
    const state = get()
    if (!state.bounds) return

    set(getCenteredCamera(DEFAULT_SCALE, state.bounds))
  },

  suppressClick: () => {
    set({ suppressClickUntil: Date.now() + CLICK_SUPPRESSION_MS })
  },

  isClickSuppressed: () => Date.now() < get().suppressClickUntil,

  worldToScreen: (point) => {
    const state = get()

    return {
      x: state.x + point.x * state.scale,
      y: state.y + point.y * state.scale,
    }
  },

  screenToWorld: (point) => {
    const state = get()

    return {
      x: (point.x - state.x) / state.scale,
      y: (point.y - state.y) / state.scale,
    }
  },
}))
