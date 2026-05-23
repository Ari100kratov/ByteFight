import { extend, useTick } from "@pixi/react"
import { Container, Graphics, Text } from "pixi.js"
import { useEffect, useRef, useState } from "react"
import type { UnitRuntime } from "../../types/UnitRuntime"
import type { StatSnapshot } from "../../types/common"

extend({ Graphics, Container, Text })

interface ResourceLayerProps {
  runtime: UnitRuntime
  x: number
  y: number
  spriteHeight: number
  cellWidth: number
  cellHeight: number
}

type ResourceChangeKind = "gain" | "loss" | null

interface AnimatedResourceRenderState {
  valueRatio: number
  lagRatio: number
  flash: number
  changeKind: ResourceChangeKind
}

function clampRatio(value: number) {
  return Math.max(0, Math.min(1, value))
}

function getRatio(stat?: StatSnapshot) {
  if (!stat || stat.max <= 0) return 0

  return clampRatio(stat.current / stat.max)
}

function getHpColor(ratio: number) {
  if (ratio <= 0.3) return 0xfb7185
  if (ratio <= 0.6) return 0xfbbf24

  return 0x34d399
}

function isDead(runtime: UnitRuntime) {
  return runtime.hp.current <= 0
}

function getResourceBarLayout({
  runtime,
  x,
  y,
  spriteHeight,
  cellWidth,
  cellHeight,
}: ResourceLayerProps) {
  const barWidth = Math.max(34, cellWidth - 12)
  const hpBarHeight = 6
  const mpBarHeight = 4
  const rawSpriteHeight = spriteHeight > 0 ? spriteHeight : cellHeight * 1.02
  const visualSpriteHeight = Math.min(
    Math.max(rawSpriteHeight, cellHeight * 0.72),
    cellHeight * 1.18,
  )
  const barYOffset = runtime.mp ? 5 : 0

  const preferredY = y - visualSpriteHeight * 0.82 - barYOffset
  const tacticalLaneY = y - cellHeight * 1.02 - barYOffset
  const hpY = Math.max(preferredY, tacticalLaneY)
  const mpY = hpY + hpBarHeight + 2
  const barX = x - barWidth / 2

  const hpText = `HP ${String(runtime.hp.current)}/${String(runtime.hp.max)}`
  const mpText = runtime.mp ? `MP ${String(runtime.mp.current)}/${String(runtime.mp.max)}` : null
  const detailsText = mpText ? `${hpText} | ${mpText}` : hpText
  const detailsX = Math.round(x)
  const detailsY = Math.round(hpY - 13)
  const detailsWidth = Math.max(46, detailsText.length * 5.4 + 10)

  return {
    barWidth,
    hpBarHeight,
    mpBarHeight,
    hpY,
    mpY,
    barX,
    detailsText,
    detailsX,
    detailsY,
    detailsWidth,
  }
}

function useAnimatedResource(targetRatio: number, lagDelayMs: number) {
  const previousTargetRef = useRef(targetRatio)
  const valueRatioRef = useRef(targetRatio)
  const lagRatioRef = useRef(targetRatio)
  const lagDelayRef = useRef(0)
  const flashRef = useRef(0)
  const changeKindRef = useRef<ResourceChangeKind>(null)
  const [renderState, setRenderState] = useState<AnimatedResourceRenderState>({
    valueRatio: targetRatio,
    lagRatio: targetRatio,
    flash: 0,
    changeKind: null,
  })

  useEffect(() => {
    const previousTarget = previousTargetRef.current
    if (Math.abs(previousTarget - targetRatio) < 0.001) return

    previousTargetRef.current = targetRatio
    flashRef.current = 1

    if (targetRatio < previousTarget) {
      valueRatioRef.current = targetRatio
      lagRatioRef.current = previousTarget
      lagDelayRef.current = lagDelayMs
      changeKindRef.current = "loss"
    } else {
      valueRatioRef.current = previousTarget
      lagRatioRef.current = targetRatio
      lagDelayRef.current = 0
      changeKindRef.current = "gain"
    }

    setRenderState({
      valueRatio: valueRatioRef.current,
      lagRatio: lagRatioRef.current,
      flash: flashRef.current,
      changeKind: changeKindRef.current,
    })
  }, [lagDelayMs, targetRatio])

  useTick((ticker) => {
    let changed = false
    const delta = ticker.deltaMS
    const valueStep = Math.min(1, delta / 220)
    const lagStep = Math.min(1, delta / 480)

    const nextValue = valueRatioRef.current + (targetRatio - valueRatioRef.current) * valueStep
    if (Math.abs(nextValue - valueRatioRef.current) > 0.001) {
      valueRatioRef.current = nextValue
      changed = true
    } else if (Math.abs(targetRatio - valueRatioRef.current) > 0) {
      valueRatioRef.current = targetRatio
      changed = true
    }

    if (lagDelayRef.current > 0) {
      lagDelayRef.current = Math.max(0, lagDelayRef.current - delta)
      changed = true
    } else {
      const nextLag = lagRatioRef.current + (targetRatio - lagRatioRef.current) * lagStep
      if (Math.abs(nextLag - lagRatioRef.current) > 0.001) {
        lagRatioRef.current = nextLag
        changed = true
      } else if (Math.abs(targetRatio - lagRatioRef.current) > 0) {
        lagRatioRef.current = targetRatio
        changed = true
      }
    }

    if (flashRef.current > 0) {
      flashRef.current = Math.max(0, flashRef.current - delta / 420)
      changed = true
    }

    if (!changed) return

    setRenderState({
      valueRatio: clampRatio(valueRatioRef.current),
      lagRatio: clampRatio(lagRatioRef.current),
      flash: flashRef.current,
      changeKind: changeKindRef.current,
    })
  })

  return renderState
}

function drawResourceBar({
  g,
  x,
  y,
  width,
  height,
  valueRatio,
  lagRatio,
  fillColor,
  lagColor,
  flashColor,
  flash,
  changeKind,
  criticalPulse,
}: {
  g: Graphics
  x: number
  y: number
  width: number
  height: number
  valueRatio: number
  lagRatio: number
  fillColor: number
  lagColor: number
  flashColor: number
  flash: number
  changeKind: ResourceChangeKind
  criticalPulse?: number
}) {
  const radius = height / 2
  const visibleValueRatio = valueRatio > 0 ? Math.max(valueRatio, 0.035) : 0
  const visibleLagRatio = lagRatio > 0 ? Math.max(lagRatio, 0.035) : 0

  g.roundRect(x, y, width, height, radius).fill({ color: 0x05070c, alpha: 0.56 })

  if (changeKind === "loss" && visibleLagRatio > visibleValueRatio) {
    g.roundRect(x, y, width * visibleLagRatio, height, radius).fill({
      color: lagColor,
      alpha: 0.38,
    })
  }

  if (visibleValueRatio > 0) {
    g.roundRect(x, y, width * visibleValueRatio, height, radius).fill({
      color: fillColor,
      alpha: 0.88,
    })

    g.rect(x, y, width * visibleValueRatio, Math.max(1, height * 0.35)).fill({
      color: 0xffffff,
      alpha: 0.14,
    })
  }

  if (changeKind === "gain" && flash > 0) {
    g.roundRect(x, y, width * visibleValueRatio, height, radius).fill({
      color: flashColor,
      alpha: flash * 0.24,
    })
  }

  if (criticalPulse && criticalPulse > 0) {
    g.roundRect(x - 1, y - 1, width + 2, height + 2, radius + 1).fill({
      color: 0xfb7185,
      alpha: criticalPulse * 0.1,
    })
  }

  g.setStrokeStyle({ width: 1, color: 0xffffff, alpha: 0.22 })
  g.roundRect(x, y, width, height, radius)
  g.stroke()
}

export function UnitBars(props: ResourceLayerProps) {
  const { runtime } = props
  const [pulse, setPulse] = useState(0)
  const [deathElapsedMs, setDeathElapsedMs] = useState(0)
  const layout = getResourceBarLayout(props)
  const dead = isDead(runtime)

  const hpRatio = getRatio(runtime.hp)
  const mpRatio = getRatio(runtime.mp)

  const hp = useAnimatedResource(hpRatio, 260)
  const mp = useAnimatedResource(mpRatio, 180)

  useTick((ticker) => {
    setPulse((value) => (value + ticker.deltaMS * 0.006) % (Math.PI * 2))

    if (!dead) {
      setDeathElapsedMs((value) => (value === 0 ? value : 0))
      return
    }

    setDeathElapsedMs((value) => Math.min(1100, value + ticker.deltaMS))
  })

  if (dead && deathElapsedMs >= 1000) return null

  const isCritical = runtime.hp.max > 0 && hp.valueRatio <= 0.3
  const criticalPulse = isCritical ? 0.35 + ((Math.sin(pulse) + 1) / 2) * 0.5 : 0
  const deathFade = dead ? 1 - clampRatio((deathElapsedMs - 720) / 280) : 1

  return (
    <pixiContainer zIndex={-1} alpha={deathFade}>
      <pixiGraphics
        draw={(g) => {
          g.clear()

          drawResourceBar({
            g,
            x: layout.barX,
            y: layout.hpY,
            width: layout.barWidth,
            height: layout.hpBarHeight,
            valueRatio: hp.valueRatio,
            lagRatio: hp.lagRatio,
            fillColor: getHpColor(hp.valueRatio),
            lagColor: 0xfca5a5,
            flashColor: 0xbbf7d0,
            flash: hp.flash,
            changeKind: hp.changeKind,
            criticalPulse,
          })

          if (runtime.mp) {
            drawResourceBar({
              g,
              x: layout.barX,
              y: layout.mpY,
              width: layout.barWidth,
              height: layout.mpBarHeight,
              valueRatio: mp.valueRatio,
              lagRatio: mp.lagRatio,
              fillColor: 0x38bdf8,
              lagColor: 0x93c5fd,
              flashColor: 0xbae6fd,
              flash: mp.flash,
              changeKind: mp.changeKind,
            })
          }
        }}
      />
    </pixiContainer>
  )
}

export function UnitResourceDetails(props: ResourceLayerProps) {
  const { runtime } = props
  const layout = getResourceBarLayout(props)

  if (isDead(runtime)) return null

  return (
    <pixiContainer zIndex={3}>
      <pixiGraphics
        draw={(g) => {
          g.clear()
          g.roundRect(
            layout.detailsX - layout.detailsWidth / 2,
            layout.detailsY - 7,
            layout.detailsWidth,
            14,
            7,
          ).fill({
            color: 0x020617,
            alpha: 0.76,
          })
          g.setStrokeStyle({ width: 1, color: 0xffffff, alpha: 0.2 })
          g.roundRect(
            layout.detailsX - layout.detailsWidth / 2,
            layout.detailsY - 7,
            layout.detailsWidth,
            14,
            7,
          )
          g.stroke()
        }}
      />

      <pixiText
        text={layout.detailsText}
        x={layout.detailsX}
        y={layout.detailsY}
        anchor={{ x: 0.5, y: 0.5 }}
        style={{
          fill: "#f8fafc",
          fontSize: 10,
          fontWeight: "700",
          stroke: { color: "#020617", width: 2 },
        }}
      />
    </pixiContainer>
  )
}
