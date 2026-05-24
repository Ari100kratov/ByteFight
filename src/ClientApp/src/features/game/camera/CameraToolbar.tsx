import { Minus, Plus, RotateCcw } from "lucide-react"
import { useGridStore } from "../state/game/grid.state.store"
import { CAMERA_ZOOM_ENABLED, useCameraStore } from "./camera.store"

export function CameraToolbar() {
  const hasLayout = useGridStore((s) => Boolean(s.layout))
  const zoomAtCenter = useCameraStore((s) => s.zoomAtCenter)
  const reset = useCameraStore((s) => s.reset)

  if (!hasLayout || !CAMERA_ZOOM_ENABLED) return null

  return (
    <div className="absolute top-3 right-3 z-10 flex items-center gap-1 rounded-md border bg-background/80 p-1 shadow-sm backdrop-blur">
      <button
        type="button"
        className="hover:bg-muted flex size-8 cursor-pointer items-center justify-center rounded-sm transition-colors"
        title="Отдалить"
        aria-label="Отдалить"
        onClick={() => {
          zoomAtCenter("out")
        }}
      >
        <Minus className="size-4" />
      </button>

      <button
        type="button"
        className="hover:bg-muted flex size-8 cursor-pointer items-center justify-center rounded-sm transition-colors"
        title="Сбросить камеру"
        aria-label="Сбросить камеру"
        onClick={reset}
      >
        <RotateCcw className="size-4" />
      </button>

      <button
        type="button"
        className="hover:bg-muted flex size-8 cursor-pointer items-center justify-center rounded-sm transition-colors"
        title="Приблизить"
        aria-label="Приблизить"
        onClick={() => {
          zoomAtCenter("in")
        }}
      >
        <Plus className="size-4" />
      </button>
    </div>
  )
}
