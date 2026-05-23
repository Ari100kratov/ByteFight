import { useEffect, useRef } from "react"
import confetti from "canvas-confetti"
import type { BattleResultTone } from "./battle-result.types"

type ConfettiOptions = NonNullable<Parameters<typeof confetti>[0]>
type ConfettiLauncher = (delayMs: number, options: ConfettiOptions) => void

const RESULT_CONFETTI_COLORS: Record<BattleResultTone, string[]> = {
  success: ["#22c55e", "#38bdf8", "#facc15", "#fb7185", "#a78bfa", "#f97316", "#f8fafc"],
  danger: ["#fb7185", "#ef4444", "#f97316", "#fed7aa", "#fef3c7"],
  warning: ["#f59e0b", "#facc15", "#fde68a", "#fb923c", "#f8fafc"],
  neutral: ["#94a3b8", "#38bdf8", "#c4b5fd", "#e2e8f0", "#64748b"],
}

function launchSuccessEffect(launch: ConfettiLauncher) {
  launch(80, {
    particleCount: 70,
    angle: 58,
    spread: 54,
    startVelocity: 48,
    gravity: 0.82,
    decay: 0.91,
    scalar: 0.9,
    origin: { x: 0.02, y: 0.72 },
  })
  launch(80, {
    particleCount: 70,
    angle: 122,
    spread: 54,
    startVelocity: 48,
    gravity: 0.82,
    decay: 0.91,
    scalar: 0.9,
    origin: { x: 0.98, y: 0.72 },
  })
  launch(420, {
    particleCount: 46,
    angle: 82,
    spread: 72,
    startVelocity: 34,
    gravity: 0.72,
    decay: 0.92,
    scalar: 0.72,
    origin: { x: 0.22, y: 0.88 },
    drift: 0.45,
  })
  launch(520, {
    particleCount: 46,
    angle: 98,
    spread: 72,
    startVelocity: 34,
    gravity: 0.72,
    decay: 0.92,
    scalar: 0.72,
    origin: { x: 0.78, y: 0.88 },
    drift: -0.45,
  })
}

function launchDefeatEffect(launch: ConfettiLauncher) {
  for (let i = 0; i < 5; i++) {
    launch(i * 180, {
      particleCount: 28,
      angle: 90,
      spread: 30 + i * 4,
      startVelocity: 32 + i * 2,
      gravity: 0.48,
      decay: 0.9,
      scalar: 0.58,
      shapes: ["circle", "star"],
      origin: { x: i % 2 === 0 ? 0.42 : 0.58, y: 1.04 },
      drift: i % 2 === 0 ? -0.25 : 0.25,
    })
  }
}

function launchWarningEffect(launch: ConfettiLauncher) {
  launch(100, {
    particleCount: 54,
    angle: 90,
    spread: 70,
    startVelocity: 26,
    gravity: 0.58,
    decay: 0.92,
    scalar: 0.62,
    shapes: ["circle"],
    origin: { x: 0.5, y: 0.86 },
  })
  launch(360, {
    particleCount: 34,
    angle: 90,
    spread: 50,
    startVelocity: 20,
    gravity: 0.46,
    decay: 0.94,
    scalar: 0.48,
    shapes: ["circle"],
    origin: { x: 0.5, y: 0.74 },
  })
}

function launchNeutralEffect(launch: ConfettiLauncher) {
  launch(120, {
    particleCount: 46,
    angle: 90,
    spread: 76,
    startVelocity: 22,
    gravity: 0.5,
    decay: 0.93,
    scalar: 0.58,
    shapes: ["circle", "star"],
    origin: { x: 0.5, y: 0.78 },
  })
}

function launchResultEffect(tone: BattleResultTone, launch: ConfettiLauncher) {
  if (tone === "success") {
    launchSuccessEffect(launch)
    return
  }

  if (tone === "danger") {
    launchDefeatEffect(launch)
    return
  }

  if (tone === "warning") {
    launchWarningEffect(launch)
    return
  }

  launchNeutralEffect(launch)
}

export function BattleResultCanvasEffects({ tone }: { tone: BattleResultTone }) {
  const canvasRef = useRef<HTMLCanvasElement | null>(null)

  useEffect(() => {
    const canvas = canvasRef.current
    if (!canvas) return

    const fire = confetti.create(canvas, {
      resize: true,
      useWorker: true,
      disableForReducedMotion: true,
    })
    const timers: number[] = []
    const colors = RESULT_CONFETTI_COLORS[tone]

    const launch: ConfettiLauncher = (delayMs, options) => {
      const timerId = window.setTimeout(() => {
        void fire({
          colors,
          ticks: 170,
          zIndex: 100,
          disableForReducedMotion: true,
          ...options,
        })
      }, delayMs)

      timers.push(timerId)
    }

    launchResultEffect(tone, launch)

    return () => {
      timers.forEach(window.clearTimeout)
      fire.reset()
    }
  }, [tone])

  return <canvas ref={canvasRef} className="battle-result-canvas-effects" aria-hidden="true" />
}
