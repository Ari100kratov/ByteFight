import { extend } from "@pixi/react"
import { Text, Ticker } from "pixi.js"
import { useEffect, useRef, useState } from "react"
import { FloatingCombatTextKind } from "../state/ui/floating.combat.text.store"

extend({ Text })

interface Props {
  value: number
  kind: FloatingCombatTextKind
  x: number
  y: number
  onComplete: () => void
}

const easeOutCubic = (t: number) => 1 - Math.pow(1 - t, 3)

const easeOutBack = (t: number) => {
  const c1 = 1.70158
  const c3 = c1 + 1

  return 1 + c3 * Math.pow(t - 1, 3) + c1 * Math.pow(t - 1, 2)
}

function getStableOffset(value: number, kind: FloatingCombatTextKind, x: number, y: number) {
  const seed = `${String(value)}:${kind}:${String(x)}:${String(y)}`
  let hash = 0
  for (let i = 0; i < seed.length; i++) {
    hash = (hash * 31 + seed.charCodeAt(i)) | 0
  }

  return ((Math.abs(hash) % 1000) / 1000 - 0.5) * 18
}

export function FloatingCombatText({ value, kind, x, y, onComplete }: Props) {
  const [state, setState] = useState({
    offsetX: 0,
    offsetY: 0,
    alpha: 1,
    scale: 1,
  })

  const elapsedRef = useRef(0)
  const startOffsetX = getStableOffset(value, kind, x, y)

  const isHealing = kind === FloatingCombatTextKind.Healing

  useEffect(() => {
    const duration = isHealing ? 1150 : 980
    const ticker = Ticker.shared

    const update = (ticker: Ticker) => {
      elapsedRef.current += ticker.deltaMS

      const t = Math.min(1, elapsedRef.current / duration)

      const moveT = easeOutCubic(t)
      const popT = Math.min(1, t / 0.28)
      const fadeT = Math.max(0, (t - 0.45) / 0.55)

      setState({
        offsetX: startOffsetX * moveT,
        offsetY: (isHealing ? -42 : -36) * moveT,
        alpha: 1 - easeOutCubic(fadeT),
        scale: (isHealing ? 0.62 : 0.72) + easeOutBack(popT) * (isHealing ? 0.52 : 0.6),
      })

      if (t >= 1) {
        ticker.remove(update)
        onComplete()
      }
    }

    ticker.add(update)

    return () => {
      ticker.remove(update)
    }
  }, [isHealing, onComplete, startOffsetX])

  return (
    <pixiText
      text={`${isHealing ? "+" : "-"}${String(value)}`}
      x={x + state.offsetX}
      y={y + state.offsetY}
      anchor={{ x: 0.5, y: 0.5 }}
      alpha={state.alpha}
      scale={state.scale}
      style={{
        fill: isHealing ? "#6ee7b7" : "#fb7185",
        fontSize: isHealing ? 20 : 22,
        fontWeight: "800",
        stroke: {
          color: isHealing ? "#064e3b" : "#3f0713",
          width: 5,
        },
        dropShadow: {
          color: "#000000",
          blur: 6,
          distance: 2,
          alpha: 0.76,
        },
      }}
    />
  )
}
