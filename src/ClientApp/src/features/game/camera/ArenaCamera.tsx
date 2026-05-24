import { useApplication } from "@pixi/react"
import type { Application } from "pixi.js"
import { useEffect, useRef, type ReactNode } from "react"
import { useGridStore } from "../state/game/grid.state.store"
import { useViewportStore } from "../state/viewport/viewport.store"
import { useArenaItemSelectionStore } from "../state/ui/arena-item.selection.store"
import { useCharacterSelectionStore } from "../state/ui/character.selection.store"
import { useEnemySelectionStore } from "../state/ui/enemy.selection.store"
import { CAMERA_ZOOM_ENABLED, useCameraStore } from "./camera.store"

interface RuntimePixiApplication {
  app?: Pick<Application, "renderer">
}

interface DragState {
  pointerId: number
  startX: number
  startY: number
  lastX: number
  lastY: number
  moved: boolean
}

interface CameraStageProps {
  children: ReactNode
}

const DRAG_THRESHOLD = 4

function clearOpenSelections() {
  useArenaItemSelectionStore.getState().clearSelection()
  useCharacterSelectionStore.getState().clearSelection()
  useEnemySelectionStore.getState().clearSelection()
}

export function CameraBoundsSync() {
  const layout = useGridStore((s) => s.layout)
  const viewport = useViewportStore((s) => s.size)
  const setBounds = useCameraStore((s) => s.setBounds)

  useEffect(() => {
    if (!layout) return
    if (viewport.width <= 0 || viewport.height <= 0) return

    setBounds({
      viewportWidth: viewport.width,
      viewportHeight: viewport.height,
      worldWidth: layout.gridPixelWidth,
      worldHeight: layout.gridPixelHeight,
    })
  }, [layout, setBounds, viewport.height, viewport.width])

  return null
}

export function CameraInput() {
  const pixiApplication = useApplication()
  const dragRef = useRef<DragState | null>(null)

  useEffect(() => {
    const renderer = (pixiApplication as RuntimePixiApplication).app?.renderer
    const canvas = renderer?.canvas
    if (!canvas) return

    const handlePointerDown = (event: PointerEvent) => {
      if (event.button !== 0) return

      dragRef.current = {
        pointerId: event.pointerId,
        startX: event.clientX,
        startY: event.clientY,
        lastX: event.clientX,
        lastY: event.clientY,
        moved: false,
      }

      canvas.setPointerCapture(event.pointerId)
    }

    const handlePointerMove = (event: PointerEvent) => {
      const drag = dragRef.current
      if (drag?.pointerId !== event.pointerId) return

      const totalDistance = Math.hypot(event.clientX - drag.startX, event.clientY - drag.startY)

      if (!drag.moved && totalDistance >= DRAG_THRESHOLD) {
        drag.moved = true
        clearOpenSelections()
        useCameraStore.getState().setDragging(true)
      }

      if (!drag.moved) return

      const delta = {
        x: event.clientX - drag.lastX,
        y: event.clientY - drag.lastY,
      }

      drag.lastX = event.clientX
      drag.lastY = event.clientY

      useCameraStore.getState().panBy(delta)
      event.preventDefault()
    }

    const finishDrag = (event: PointerEvent) => {
      const drag = dragRef.current
      if (drag?.pointerId !== event.pointerId) return

      if (drag.moved) {
        useCameraStore.getState().suppressClick()
      }

      useCameraStore.getState().setDragging(false)
      dragRef.current = null

      if (canvas.hasPointerCapture(event.pointerId)) {
        canvas.releasePointerCapture(event.pointerId)
      }
    }

    const handleWheel = (event: WheelEvent) => {
      const rect = canvas.getBoundingClientRect()

      useCameraStore.getState().zoomAt(
        {
          x: event.clientX - rect.left,
          y: event.clientY - rect.top,
        },
        event.deltaY < 0 ? "in" : "out",
      )

      event.preventDefault()
    }

    canvas.addEventListener("pointerdown", handlePointerDown)
    canvas.addEventListener("pointermove", handlePointerMove)
    canvas.addEventListener("pointerup", finishDrag)
    canvas.addEventListener("pointercancel", finishDrag)
    if (CAMERA_ZOOM_ENABLED) {
      canvas.addEventListener("wheel", handleWheel, { passive: false })
    }

    return () => {
      canvas.removeEventListener("pointerdown", handlePointerDown)
      canvas.removeEventListener("pointermove", handlePointerMove)
      canvas.removeEventListener("pointerup", finishDrag)
      canvas.removeEventListener("pointercancel", finishDrag)
      if (CAMERA_ZOOM_ENABLED) {
        canvas.removeEventListener("wheel", handleWheel)
      }
    }
  }, [pixiApplication])

  return null
}

export function CameraStage({ children }: CameraStageProps) {
  const x = useCameraStore((s) => s.x)
  const y = useCameraStore((s) => s.y)
  const scale = useCameraStore((s) => s.scale)

  return (
    <pixiContainer x={x} y={y} scale={scale} sortableChildren={true}>
      {children}
    </pixiContainer>
  )
}
