import { Badge } from '@/components/ui/badge'
import { cn } from '@/shared/lib/utils'

import { useBattleStore } from '../state/battle.store'
import { statusMeta, TERRAIN_HINTS, TERRAIN_NAMES, type BattleUnitDto } from '../types'

/** Панель раунда и очереди ходов по инициативе. */
export function TurnQueuePanel() {
  const state = useBattleStore((s) => s.state)

  if (!state) return null

  const order = state.turnOrder
    .map((id) => state.units.find((u) => u.id === id))
    .filter((u): u is BattleUnitDto => Boolean(u))
    .slice(0, 8)

  return (
    <div className="flex flex-col gap-2 rounded-xl border border-[#2c3a24] bg-[#161f12]/85 p-3 backdrop-blur">
      <div className="flex items-center justify-between">
        <span className="text-sm font-semibold text-[#e8d9a0]">Раунд {state.round}</span>
        <span className="text-xs text-[#a8a88f]">Порядок хода</span>
      </div>

      <div className="flex flex-col gap-1">
        {order.map((unit) => (
          <div
            key={unit.id}
            className={cn(
              'flex items-center justify-between rounded-md px-2 py-1 text-xs',
              unit.id === state.activeUnitId
                ? 'bg-[#2c3a24] text-[#e8d9a0]'
                : unit.isPlayerSide
                  ? 'text-[#d9c07a]'
                  : 'text-[#c98070]',
            )}
          >
            <span>{unit.name}</span>
            <span className="text-[10px] text-[#a8a88f]">⚡{unit.initiative}</span>
          </div>
        ))}
      </div>
    </div>
  )
}

/** Инспектор выбранного юнита: характеристики, статусы, способности. */
export function UnitInspector() {
  const selectedId = useBattleStore((s) => s.selectedUnitId)
  const unit = useBattleStore((s) => s.unitById(s.selectedUnitId))
  const state = useBattleStore((s) => s.state)

  if (!unit) {
    return (
      <div className="rounded-xl border border-[#2c3a24] bg-[#161f12]/85 p-3 text-xs text-[#a8a88f] backdrop-blur">
        Кликните по юниту, чтобы увидеть его подробности.
      </div>
    )
  }

  return (
    <div className="flex flex-col gap-2 rounded-xl border border-[#2c3a24] bg-[#161f12]/85 p-3 text-xs backdrop-blur">
      <div className="flex items-center justify-between">
        <span className={cn('text-sm font-semibold', unit.isPlayerSide ? 'text-[#d9c07a]' : 'text-[#c98070]')}>
          {unit.name}
        </span>
        <button type="button" className="text-[#a8a88f] hover:text-[#e8d9a0]" onClick={() => { useBattleStore.getState().selectUnit(null); }}>
          ×
        </button>
      </div>

      <div className="flex flex-wrap gap-1.5">
        <Badge variant="outline" className="border-[#4a5a3a] text-[#c9c9b0]">
          ❤ {Math.max(0, Math.round(unit.health))}/{Math.round(unit.maxHealth)}
        </Badge>
        {unit.maxMana > 0 && (
          <Badge variant="outline" className="border-[#4a5a3a] text-[#c9c9b0]">
            ⚗ {Math.floor(unit.mana)}/{Math.floor(unit.maxMana)}
          </Badge>
        )}
        <Badge variant="outline" className="border-[#4a5a3a] text-[#c9c9b0]">
          🛡 {unit.initiative} иниц.
        </Badge>
      </div>

      {unit.statuses.length > 0 && (
        <div className="flex flex-wrap gap-1">
          {unit.statuses.map((status, index) => {
            return (
              <span
                key={`${String(status.type)}-${String(index)}`}
                className={cn(
                  'rounded px-1.5 py-0.5 text-[10px]',
                  statusMeta(status.type).kind === 'good'
                    ? 'bg-[#2c4a2a] text-[#9fe06a]'
                    : 'bg-[#4a2626] text-[#ff8a6a]',
                )}
              >
                {statusMeta(status.type).name} · {String(status.turns)}х
              </span>
            )
          })}
        </div>
      )}

      <div className="flex flex-col gap-1">
        {unit.abilities.map((ability) => (
          <div key={ability.type} className="flex items-center justify-between text-[11px] text-[#c9c9b0]">
            <span>{ability.name}</span>
            <span className="text-[10px] text-[#a8a88f]">
              {ability.damage > 0 && `⚔${String(ability.damage)} `}
              {ability.healing > 0 && `✚${String(ability.healing)}`} 
              {ability.range > 0 && `→${String(ability.range)}`}
            </span>
          </div>
        ))}
      </div>

      {state && <TerrainHint hex={unit.position} state={state} selectedId={selectedId} />}
    </div>
  )
}

function TerrainHint({
  hex,
  state,
}: {
  hex: { x: number; y: number }
  state: NonNullable<ReturnType<typeof useBattleStore.getState>['state']>
  selectedId: string | null
}) {
  const cell = state.arena.terrain.find((t) => t.x === hex.x && t.y === hex.y)
  const terrain = cell ? cell.terrain : 1

  return (
    <div className="mt-1 rounded-md bg-[#11180f]/70 px-2 py-1 text-[10px] text-[#a8a88f]">
      Гекс ({String(hex.x)}, {String(hex.y)}) — {TERRAIN_NAMES[terrain] ?? ''}.{' '}
      {TERRAIN_HINTS[terrain] ?? ''}
    </div>
  )
}
