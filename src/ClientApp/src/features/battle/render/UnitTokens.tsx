import { extend } from '@pixi/react'
import { useMemo } from 'react'
import { Container, Graphics, Text, TextStyle } from 'pixi.js'

import { COLORS } from './battleColors'
import { hexToPixel } from '../hex'
import { useBattleStore } from '../state/battle.store'
import { statusMeta, type BattleUnitDto } from '../types'

extend({ Container, Graphics, Text })

interface UnitTokensProps {
  hexSize: number
}

const TOKEN_RADIUS = 0.62

/**
 * Юниты поля: опознавательные знаки с 8-направленным клином поворота,
 * полосами здоровья/маны, метками статусов и кольцом активного юнита.
 * Процедурная графика служит палитр-заменой спрайтов: когда арт
 * подгружен, токен рисуется под ним.
 */
export function UnitTokens({ hexSize }: UnitTokensProps) {
  const units = useBattleStore((s) => s.state?.units)
  const activeUnitId = useBattleStore((s) => s.state?.activeUnitId)
  const selectUnit = useBattleStore((s) => s.selectUnit)
  const selectedUnitId = useBattleStore((s) => s.selectedUnitId)

  const visible = useMemo(() => units?.filter((u) => !u.isDead) ?? [], [units])

  return (
    <pixiContainer sortableChildren>
      {visible.map((unit) => (
        <UnitToken
          key={unit.id}
          unit={unit}
          hexSize={hexSize}
          isActive={unit.id === activeUnitId}
          isSelected={unit.id === selectedUnitId}
          onSelect={() => { selectUnit(unit.id); }}
        />
      ))}
    </pixiContainer>
  )
}

interface UnitTokenProps {
  unit: BattleUnitDto
  hexSize: number
  isActive: boolean
  isSelected: boolean
  onSelect: () => void
}

function UnitToken({ unit, hexSize, isActive, isSelected, onSelect }: UnitTokenProps) {
  const center = hexToPixel(unit.position, hexSize)
  const radius = hexSize * TOKEN_RADIUS

  // Угол клина поворота: 8 направлений как на сервере.
  const facingAngle = (unit.facing - 1) * 45 * (Math.PI / 180)

  const draw = (g: Graphics) => {
    g.clear()

    const plate = unit.isPlayerSide ? COLORS.playerPlate : COLORS.enemyPlate
    const dark = unit.isPlayerSide ? COLORS.playerPlateDark : COLORS.enemyPlateDark

    // Круглое основание с окантовкой.
    g.circle(0, 0, radius)
    g.fill({ color: dark })
    g.circle(0, 0, radius * 0.88)
    g.fill({ color: plate })
    g.circle(0, 0, radius * 0.7)
    g.fill({ color: dark, alpha: 0.55 })

    // Клин направления взгляда.
    const wedgeLength = radius * 1.18
    g.moveTo(Math.cos(facingAngle) * wedgeLength, Math.sin(facingAngle) * wedgeLength)
    g.lineTo(
      Math.cos(facingAngle + 2.5) * radius * 0.55,
      Math.sin(facingAngle + 2.5) * radius * 0.55,
    )
    g.lineTo(
      Math.cos(facingAngle - 2.5) * radius * 0.55,
      Math.sin(facingAngle - 2.5) * radius * 0.55,
    )
    g.closePath()
    g.fill({ color: 0xf2e7c9 })

    // Полоса здоровья.
    const barWidth = radius * 1.7
    const barY = radius + 6
    g.roundRect(-barWidth / 2, barY, barWidth, 5, 2)
    g.fill({ color: COLORS.hpBack })
    const hpRatio = Math.max(0, Math.min(1, unit.health / Math.max(1, unit.maxHealth)))
    g.roundRect(-barWidth / 2, barY, barWidth * hpRatio, 5, 2)
    g.fill({ color: hpRatio < 0.35 ? COLORS.hpLow : COLORS.hpFill })

    // Полоса маны, если есть.
    if (unit.maxMana > 0) {
      const manaY = barY + 7
      g.roundRect(-barWidth / 2, manaY, barWidth, 3, 1.5)
      g.fill({ color: COLORS.hpBack })
      const manaRatio = Math.max(0, Math.min(1, unit.mana / Math.max(1, unit.maxMana)))
      g.roundRect(-barWidth / 2, manaY, barWidth * manaRatio, 3, 1.5)
      g.fill({ color: COLORS.manaFill })
    }

    // Метки статусов: точки поверх токена.
    unit.statuses.slice(0, 5).forEach((status, i) => {
      const isGood = statusMeta(status.type).kind === 'good'
      g.circle(-radius + 6 + i * 7, -radius - 4, 2.4)
      g.fill({ color: isGood ? 0x9fe06a : 0xc2543f })
    })

    // Кольцо активного юнита / выбор.
    if (isActive || isSelected) {
      g.circle(0, 0, radius * 1.32)
      g.stroke({
        width: isActive ? 3 : 2,
        color: COLORS.activeRing,
        alpha: isActive ? 0.95 : 0.55,
      })
    }
  }

  return (
    <pixiContainer
      x={center.x}
      y={center.y}
      sortableChildren
      eventMode="static"
      cursor="pointer"
      onPointerTap={onSelect}
    >
      <pixiGraphics draw={draw} />

      <pixiText
        text={unit.name}
        anchor={{ x: 0.5, y: 1 }}
        y={-radius - 6}
        style={new TextStyle({
          fontFamily: 'Georgia, serif',
          fontSize: Math.max(11, hexSize * 0.32),
          fill: 0xe8d9a0,
          stroke: { color: 0x11180f, width: 3 },
        })}
      />
    </pixiContainer>
  )
}
