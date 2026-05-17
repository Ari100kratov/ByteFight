import { useEffect, useMemo, useState } from "react"
import { Application, extend } from "@pixi/react"
import { ActionType, type ActionAssetDto } from "@/shared/types/action"
import { AnimatedSprite, type Texture } from "pixi.js"
import { loadActionAssets } from "@/shared/api/loadActionAssets"
import { Skeleton } from "@/components/ui/skeleton"
import { AbilityType, type AbilityDto } from "@/shared/types/ability"

extend({ AnimatedSprite })

type Props = {
  actionAssets: ActionAssetDto[]
  abilities: AbilityDto[]
}

export function SpriteAnimationPlayer({ actionAssets, abilities = [] }: Props) {
  const [textures, setTextures] = useState<Texture[]>([])
  const [loading, setLoading] = useState(false)

  const previewAssets = useMemo(
    () => createPreviewAssets(actionAssets, abilities),
    [actionAssets, abilities],
  )

  useEffect(() => {
    let cancelled = false
    let loadedTextures: Texture[] = []

    if (previewAssets.length === 0) {
      setTextures([])
      setLoading(false)
      return
    }

    setLoading(false)

    void loadActionAssets(previewAssets)
      .then((frames) => {
        loadedTextures = frames
        if (!cancelled) {
          setTextures(frames)
        }
      })
      .catch(console.error)
      .finally(() => {
        if (!cancelled) {
          setLoading(true)
        }
      })

    return () => {
      cancelled = true
      for (const texture of loadedTextures) {
        texture.destroy(false)
      }
      setTextures([])
    }
  }, [previewAssets])

  const width = 210
  const height = 150
  const first = previewAssets[0]?.spriteAnimation

  return (
    <div className="relative h-[155px] w-[215px]">
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
        <div className="absolute top-0 left-0 h-full w-full">
          <Skeleton className="h-full w-full" />
        </div>
      )}
    </div>
  )
}

function createPreviewAssets(
  actionAssets: ActionAssetDto[],
  abilities: AbilityDto[],
): ActionAssetDto[] {
  const result: ActionAssetDto[] = []

  pushActionAssets(result, actionAssets, ActionType.Idle)
  pushActionAssets(result, actionAssets, ActionType.Walk)
  pushActionAssets(result, actionAssets, ActionType.Run)

  pushAbilityAssets(result, abilities, AbilityType.BasicRangedAttack)
  pushAbilityAssets(result, abilities, AbilityType.BasicMeleeAttack)

  return result
}

function pushActionAssets(
  result: ActionAssetDto[],
  assets: ActionAssetDto[],
  actionType: ActionType,
) {
  result.push(
    ...assets.filter((x) => x.actionType === actionType).sort((a, b) => a.variant - b.variant),
  )
}

function pushAbilityAssets(
  result: ActionAssetDto[],
  abilities: AbilityDto[],
  abilityType: AbilityType,
) {
  const ability = abilities.find((x) => x.type === abilityType)
  if (!ability) return

  result.push(
    ...ability.actionAssets
      .filter((x) => x.actionType === ActionType.Attack)
      .sort((a, b) => a.variant - b.variant),
  )
}
