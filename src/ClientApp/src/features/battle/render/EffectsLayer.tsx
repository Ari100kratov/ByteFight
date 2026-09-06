import { extend } from '@pixi/react'
import { useEffect, useRef, useState } from 'react'
import { Container, Text, TextStyle } from 'pixi.js'

import { COLORS } from './battleColors'
import { hexToPixel } from '../hex'
import { useBattleStore } from '../state/battle.store'
import { statusMeta, type BattleLogEntry } from '../types'

extend({ Container, Text })

interface FloatingText {
  id: string
  x: number
  y: number
  text: string
  color: string
  born: number
  age: number
}

const LIFETIME_MS = 1400
const RISE_SPEED = 30
const TICK_MS = 50

/**
 * Всплывающий боевой текст: урон, лечение, метки статусов.
 * Полностью управляется таймером — рендер остаётся чистым.
 */
export function EffectsLayer({ hexSize }: { hexSize: number }) {
  const [texts, setTexts] = useState<FloatingText[]>([])
  const seen = useRef(new Set<string>())

  useEffect(() => {
    const timer = window.setInterval(() => {
      const { logs, state } = useBattleStore.getState()
      const now = performance.now()
      const fresh: FloatingText[] = []

      for (const entry of logs) {
        if (seen.current.has(entry.id)) continue
        seen.current.add(entry.id)

        const spawned = spawnFromEntry(entry, state?.units ?? [], hexSize)
        if (spawned) {
          fresh.push(spawned)
        }
      }

      setTexts((previous) => {
        const updated = [...previous, ...fresh]
          .map((t) => ({ ...t, age: Math.min(1, (now - t.born) / LIFETIME_MS) }))
          .filter((t) => t.age < 1)

        return updated.length === previous.length && fresh.length === 0 ? previous : updated
      })
    }, TICK_MS)

    return () => { window.clearInterval(timer) }
  }, [hexSize])

  if (texts.length === 0) {
    return null
  }

  return (
    <pixiContainer>
      {texts.map((t) => (
        <pixiText
          key={t.id}
          text={t.text}
          x={t.x}
          y={t.y - t.age * RISE_SPEED}
          anchor={{ x: 0.5, y: 0.5 }}
          alpha={1 - t.age * t.age}
          style={
            new TextStyle({
              fontFamily: 'Georgia, serif',
              fontSize: Math.max(14, hexSize * 0.42),
              fontWeight: '700',
              fill: t.color,
              stroke: { color: '#11180f', width: 3 },
            })
          }
        />
      ))}
    </pixiContainer>
  )
}

interface UnitLike {
  id: string
  position: { x: number; y: number }
}

function spawnFromEntry(
  entry: BattleLogEntry,
  units: readonly UnitLike[],
  hexSize: number,
): FloatingText | null {
  let position: { x: number; y: number } | null = null
  let text = ''
  let color: string = COLORS.infoText

  if (entry.entryType === 'AbilityUsedLogEntryDto') {
    const target = units.find((u) => u.id === entry.targetId)
    if (target) {
      position = target.position
      const isHeal = entry.effectType === 2
      text = `${isHeal ? '+' : '-'}${String(Math.round(entry.value))}`
      color = isHeal ? COLORS.healText : COLORS.damageText
    }
  } else if (entry.entryType === 'StatusAppliedLogEntryDto') {
    const target = units.find((u) => u.id === entry.targetId)
    if (target) {
      position = target.position
      text = `(${String(entry.duration)})`
      color = statusMeta(entry.statusType).kind === 'good' ? COLORS.healText : COLORS.statusText
    }
  }

  if (!position) {
    return null
  }

  const pixel = hexToPixel(position, hexSize)

  return {
    id: entry.id,
    x: pixel.x + (Math.random() - 0.5) * hexSize * 0.6,
    y: pixel.y - hexSize * 0.4,
    text,
    color,
    born: performance.now(),
    age: 0,
  }
}
