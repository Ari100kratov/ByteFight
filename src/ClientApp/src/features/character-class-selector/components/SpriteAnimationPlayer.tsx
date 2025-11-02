import { useEffect, useState } from "react"
import { Application } from "@pixi/react"
import { ActionType, type ActionAssetDto } from "@/shared/types/action"
import type { Texture } from "pixi.js"
import { loadActionAssets } from "@/shared/api/loadActionAssets"
import { Skeleton } from "@/components/ui/skeleton"

type Props = {
  actionAssets: ActionAssetDto[]
}

export function SpriteAnimationPlayer({ actionAssets }: Props) {
  const [textures, setTextures] = useState<Texture[]>([])
  const [loading, setLoading] = useState(false)

  useEffect(() => {
    let cancelled = false
    if (actionAssets.length === 0) return

    const filter = [ActionType.Idle, ActionType.Walk, ActionType.Run, ActionType.Attack]

    setLoading(false)
    loadActionAssets(actionAssets, filter)
      .then((frames) => {
        if (!cancelled) setTextures(frames)
      })
      .finally(() => !cancelled && setLoading(true))

    return () => {
      cancelled = true
      for (const t of textures) t.destroy(false)
      setTextures([])
    }
  }, [actionAssets])

  const width = 210
  const height = 150
  const first = actionAssets[0]?.spriteAnimation

  return (
    // TODO: плохо сделано из-за того, что если делать по-другому - Application моргает черным цветом, при первом рендере
    <div className="relative w-[215px] h-[155px]">
      <Application width={width} height={height} background="#f9fafb">
        {loading && first && textures.length > 0 && (
          <pixiAnimatedSprite
            ref={(ref) => ref?.play()}
            textures={textures}
            x={width / 2}
            y={height / 2}
            anchor={{ x: 0.5, y: 0.7 }}
            scale={{
              x: first.scale.x * 1.5,
              y: first.scale.y * 1.5,
            }}
            animationSpeed={first.animationSpeed}
            autoPlay
            loop
          />
        )}
      </Application>

      {!loading && (
        <div className="absolute top-0 left-0 w-full h-full">
          <Skeleton className="w-full h-full" />
        </div>
      )}
    </div>
  )
}
