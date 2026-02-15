import { useApplication } from "@pixi/react"
import { useEffect } from "react"
import { useViewportStore } from "./state/viewport/viewport.store"

export function ResizeHandler() {
  const app = useApplication()
  const { width, height } = useViewportStore(s => s.size)

  useEffect(() => {
    if (!app)
      return

    if (width === 0 || height === 0)
      return

    const renderer = app.app.renderer
    if (renderer.width === width && renderer.height === height)
      return

    renderer.resize(width, height)
  }, [app, width, height])

  return null
}