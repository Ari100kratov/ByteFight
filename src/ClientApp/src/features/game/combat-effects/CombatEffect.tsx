import { extend } from "@pixi/react"
import { Container, Graphics, Ticker } from "pixi.js"
import { useEffect, useMemo, useRef, useState } from "react"
import { CombatVisualEffectKind, type CombatVisualEffectKind as EffectKind } from "../state/ui/combat.effects.store"

extend({ Container, Graphics })

interface CombatEffectPoint {
  x: number
  bodyY: number
  groundY: number
}

interface Props {
  kind: EffectKind
  point: CombatEffectPoint
  seed: string
  onComplete: () => void
}

interface Particle {
  angle: number
  distance: number
  delay: number
  size: number
  drift: number
}

const EFFECT_DURATION: Record<EffectKind, number> = {
  [CombatVisualEffectKind.DamageImpact]: 520,
  [CombatVisualEffectKind.HealingPulse]: 920,
  [CombatVisualEffectKind.ItemPickup]: 760,
}

function easeOutCubic(t: number) {
  return 1 - Math.pow(1 - t, 3)
}

function easeOutBack(t: number) {
  const c1 = 1.70158
  const c3 = c1 + 1

  return 1 + c3 * Math.pow(t - 1, 3) + c1 * Math.pow(t - 1, 2)
}

function seededValue(seed: string, salt: number) {
  let hash = salt

  for (let i = 0; i < seed.length; i++) {
    hash = (hash * 31 + seed.charCodeAt(i) + salt) | 0
  }

  return (Math.abs(hash) % 1000) / 1000
}

function createParticles(seed: string, count: number): Particle[] {
  return Array.from({ length: count }, (_, index) => ({
    angle: -Math.PI * (0.15 + seededValue(seed, index + 1) * 0.7),
    distance: 20 + seededValue(seed, index + 17) * 30,
    delay: seededValue(seed, index + 31) * 0.28,
    size: 2 + seededValue(seed, index + 47) * 2.5,
    drift: (seededValue(seed, index + 61) - 0.5) * 22,
  }))
}

function drawDamageImpact(g: Graphics, point: CombatEffectPoint, t: number) {
  const alpha = 1 - easeOutCubic(t)
  const ringT = easeOutCubic(Math.min(1, t / 0.78))
  const flashT = Math.min(1, t / 0.38)

  g.ellipse(point.x, point.groundY - 4, 13 + ringT * 32, 4 + ringT * 12).fill({
    color: 0xfb7185,
    alpha: alpha * 0.16,
  })
  g.setStrokeStyle({ width: 2, color: 0xff4d6d, alpha: alpha * 0.72 })
  g.ellipse(point.x, point.groundY - 4, 15 + ringT * 36, 5 + ringT * 12)
  g.stroke()

  g.circle(point.x, point.bodyY, 10 + easeOutBack(flashT) * 18).fill({
    color: 0xff335f,
    alpha: alpha * 0.18,
  })
  g.setStrokeStyle({ width: 2, color: 0xffe4ea, alpha: alpha * 0.56 })

  for (let i = 0; i < 5; i++) {
    const angle = -Math.PI * 0.92 + i * 0.46
    const inner = 8 + ringT * 12
    const outer = 26 + ringT * 24

    g.moveTo(point.x + Math.cos(angle) * inner, point.bodyY + Math.sin(angle) * inner)
    g.lineTo(point.x + Math.cos(angle) * outer, point.bodyY + Math.sin(angle) * outer)
  }

  g.stroke()
}

function drawHealingPulse(g: Graphics, point: CombatEffectPoint, t: number, particles: Particle[]) {
  const alpha = 1 - easeOutCubic(t)
  const ringT = easeOutCubic(Math.min(1, t / 0.86))
  const bloomT = Math.min(1, t / 0.34)

  g.circle(point.x, point.bodyY, 12 + easeOutBack(bloomT) * 18).fill({
    color: 0x34d399,
    alpha: alpha * 0.12,
  })
  g.setStrokeStyle({ width: 2, color: 0x86efac, alpha: alpha * 0.64 })
  g.circle(point.x, point.bodyY, 16 + ringT * 34)
  g.stroke()
  g.setStrokeStyle({ width: 1, color: 0xfacc15, alpha: alpha * 0.48 })
  g.circle(point.x, point.bodyY, 9 + ringT * 24)
  g.stroke()

  for (const particle of particles) {
    const localT = Math.min(1, Math.max(0, (t - particle.delay) / (1 - particle.delay)))
    if (localT <= 0) continue

    const moveT = easeOutCubic(localT)
    const x = point.x + Math.cos(particle.angle) * particle.distance * 0.35 + particle.drift * moveT
    const y = point.bodyY + Math.sin(particle.angle) * particle.distance * moveT - 18 * moveT

    g.circle(x, y, particle.size * (1 - localT * 0.45)).fill({
      color: 0xbbf7d0,
      alpha: (1 - localT) * 0.82,
    })
  }
}

function drawItemPickup(g: Graphics, point: CombatEffectPoint, t: number, particles: Particle[]) {
  const alpha = 1 - easeOutCubic(t)
  const ringT = easeOutCubic(Math.min(1, t / 0.72))

  g.setStrokeStyle({ width: 2, color: 0xfacc15, alpha: alpha * 0.74 })
  g.ellipse(point.x, point.groundY, 12 + ringT * 42, 5 + ringT * 14)
  g.stroke()
  g.circle(point.x, point.bodyY, 8 + ringT * 20).fill({
    color: 0xfbbf24,
    alpha: alpha * 0.13,
  })

  for (const particle of particles) {
    const localT = Math.min(1, Math.max(0, (t - particle.delay * 0.75) / 0.78))
    if (localT <= 0) continue

    const moveT = easeOutCubic(localT)
    const x = point.x + Math.cos(particle.angle) * particle.distance * moveT
    const y = point.bodyY + Math.sin(particle.angle) * particle.distance * moveT
    const size = particle.size * (1 - localT * 0.35)

    g.setStrokeStyle({ width: 1.4, color: 0xfef3c7, alpha: (1 - localT) * 0.82 })
    g.moveTo(x - size, y)
    g.lineTo(x + size, y)
    g.moveTo(x, y - size)
    g.lineTo(x, y + size)
    g.stroke()
  }
}

export function CombatEffect({ kind, point, seed, onComplete }: Props) {
  const [progress, setProgress] = useState(0)
  const elapsedRef = useRef(0)
  const particles = useMemo(() => createParticles(seed, kind === CombatVisualEffectKind.DamageImpact ? 5 : 9), [kind, seed])

  useEffect(() => {
    const ticker = Ticker.shared
    const duration = EFFECT_DURATION[kind]

    const update = (ticker: Ticker) => {
      elapsedRef.current += ticker.deltaMS
      const nextProgress = Math.min(1, elapsedRef.current / duration)

      setProgress(nextProgress)

      if (nextProgress >= 1) {
        ticker.remove(update)
        onComplete()
      }
    }

    ticker.add(update)

    return () => {
      ticker.remove(update)
    }
  }, [kind, onComplete])

  return (
    <pixiContainer zIndex={100000}>
      <pixiGraphics
        draw={(g) => {
          g.clear()

          if (kind === CombatVisualEffectKind.DamageImpact) {
            drawDamageImpact(g, point, progress)
            return
          }

          if (kind === CombatVisualEffectKind.HealingPulse) {
            drawHealingPulse(g, point, progress, particles)
            return
          }

          drawItemPickup(g, point, progress, particles)
        }}
      />
    </pixiContainer>
  )
}
