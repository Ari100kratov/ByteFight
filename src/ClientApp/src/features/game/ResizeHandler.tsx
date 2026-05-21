import { useApplication } from "@pixi/react"
import { useEffect } from "react"
import type { Application } from "pixi.js"
import { useViewportStore } from "./state/viewport/viewport.store"

interface RuntimePixiApplication {
  app?: Pick<Application, "renderer">
}

export function ResizeHandler() {
  const pixiApplication = useApplication()
  const { width, height } = useViewportStore((s) => s.size)

  useEffect(() => {
    const renderer = (pixiApplication as RuntimePixiApplication).app?.renderer
    if (!renderer) return

    if (width === 0 || height === 0) return

    if (renderer.width === width && renderer.height === height) return

    renderer.resize(width, height)
  }, [pixiApplication, width, height])

  return null
}
