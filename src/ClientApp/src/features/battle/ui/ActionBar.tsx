import { Button } from '@/components/ui/button'
import { Badge } from '@/components/ui/badge'
import { cn } from '@/shared/lib/utils'

import { battleHub } from '../api/battleHub'
import { useBattleStore } from '../state/battle.store'
import { ABILITY_EFFECT_NAMES, ABILITY_SHAPE_NAMES, statusMeta, type BattleAbilityDto } from '../types'

interface ActionBarProps {
  sessionId: string
}

/**
 * Панель действий игрока: очки хода, способности с ресурсами
 * и кнопка завершения хода.
 */
export function ActionBar({ sessionId }: ActionBarProps) {
  const player = useBattleStore((s) => s.playerUnit())
  const isPlayerTurn = useBattleStore((s) => s.isPlayerTurn())
  const targeting = useBattleStore((s) => s.targeting)
  const beginTargeting = useBattleStore((s) => s.beginTargeting)

  if (!player) {
    return <div className="rounded-xl border border-[#2c3a24] bg-[#161f12]/80 p-4 text-sm text-[#c9c9b0]">Герой погиб...</div>
  }

  const canAct = isPlayerTurn && player.actions > 0

  return (
    <div className="flex flex-col gap-3 rounded-xl border border-[#2c3a24] bg-[#161f12]/85 p-4 backdrop-blur">
      <div className="flex items-center justify-between gap-2">
        <div className="flex items-center gap-2 text-sm text-[#e8d9a0]">
          <Badge variant="outline" className="border-[#9c7b3a] text-[#e8d9a0]">
            ⚗ {Math.floor(player.mana)}/{Math.floor(player.maxMana)}
          </Badge>
          <Badge variant="outline" className="border-[#9c7b3a] text-[#e8d9a0]">
            👣 {player.movePoints}
          </Badge>
          <Badge variant="outline" className="border-[#9c7b3a] text-[#e8d9a0]">
            ⚡ {player.actions}/2
          </Badge>
        </div>

        <Button
          size="sm"
          variant="outline"
          disabled={!isPlayerTurn}
          className="border-[#9c7b3a] text-[#e8d9a0] hover:bg-[#2c3a24]"
          onClick={() => void battleHub.submitEndTurn(sessionId)}
        >
          Завершить ход
        </Button>
      </div>

      <div className="grid grid-cols-2 gap-2 lg:grid-cols-4">
        {player.abilities.map((ability) => (
          <AbilityButton
            key={ability.type}
            ability={ability}
            cooldown={player.cooldowns[String(ability.type)] ?? 0}
            disabled={!canAct || (player.cooldowns[String(ability.type)] ?? 0) > 0 || ability.manaCost > player.mana}
            selected={targeting?.ability.type === ability.type}
            onSelect={() =>
              { beginTargeting(
                targeting?.ability.type === ability.type
                  ? null
                  : { ability, targetHexes: [] },
              ); }
            }
          />
        ))}
      </div>

      {targeting && (
        <div className="text-xs text-[#c9c9b0]">
          Выберите гекс на поле для «{targeting.ability.name}». Повторный клик по кнопке отменяет.
        </div>
      )}
    </div>
  )
}

interface AbilityButtonProps {
  ability: BattleAbilityDto
  cooldown: number
  disabled: boolean
  selected: boolean
  onSelect: () => void
}

function AbilityButton({ ability, cooldown, disabled, selected, onSelect }: AbilityButtonProps) {
  const description = describeAbility(ability)

  return (
    <button
      type="button"
      disabled={disabled}
      onClick={onSelect}
      title={description}
      className={cn(
        'group relative flex min-h-20 flex-col items-start gap-1 rounded-lg border p-2 text-left transition-colors',
        selected
          ? 'border-[#e8d9a0] bg-[#2c3a24]'
          : 'border-[#3a4a30] bg-[#1a2415] hover:border-[#9c7b3a] hover:bg-[#22301c]',
        disabled && 'cursor-not-allowed opacity-45 hover:border-[#3a4a30] hover:bg-[#1a2415]',
      )}
    >
      <span className="text-sm font-semibold text-[#e8d9a0]">{ability.name}</span>
      <span className="text-[11px] leading-tight text-[#c9c9b0]">
        {ABILITY_EFFECT_NAMES[ability.effectType]}
        {ability.range > 0 && ` · дальность ${String(ability.range)}`}
        {ability.shape !== 1 && ` · ${ABILITY_SHAPE_NAMES[ability.shape] ?? ''}`}
      </span>
      <span className="mt-auto flex flex-wrap gap-1 text-[10px] text-[#a8a88f]">
        {ability.manaCost > 0 && <span>⚗ {ability.manaCost}</span>}
        {ability.cooldown > 0 && <span>⏳ {ability.cooldown}</span>}
        <span>⚡ {ability.actionCost}</span>
      </span>

      {cooldown > 0 && (
        <span className="absolute right-1.5 top-1.5 rounded bg-[#11180f]/85 px-1.5 py-0.5 text-[10px] text-[#ff8a6a]">
          {cooldown}
        </span>
      )}
    </button>
  )
}

function describeAbility(ability: BattleAbilityDto): string {
  const parts: string[] = []

  if (ability.damage > 0) parts.push(`Урон: ${String(ability.damage)}`)
  if (ability.healing > 0) parts.push(`Лечение: ${String(ability.healing)}`)
  if (ability.range > 0) parts.push(`Дальность: ${String(ability.range)}`)
  if (ability.areaRadius > 0) parts.push(`Радиус: ${String(ability.areaRadius)}`)
  if (ability.dashesToTarget) parts.push('Рывок к цели')
  if (ability.manaCost > 0) parts.push(`Мана: ${String(ability.manaCost)}`)
  if (ability.cooldown > 0) parts.push(`Перезарядка: ${String(ability.cooldown)}`)

  for (const status of ability.statuses) {
    const meta = statusMeta(status.type)
    {
      parts.push(`${meta.name}: ${String(status.duration)} х., сила ${String(status.magnitude)}`)
    }
  }

  return parts.join('\n')
}
