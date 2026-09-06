import { cn } from '@/shared/lib/utils'

import { useBattleStore } from '../state/battle.store'
import { statusMeta, type BattleLogEntry } from '../types'

/** Журнал боя в живом, славянском духе. */
export function BattleLogPanel() {
  const logs = useBattleStore((s) => s.logs)

  return (
    <div className="flex h-full min-h-0 flex-col rounded-xl border border-[#2c3a24] bg-[#161f12]/85 backdrop-blur">
      <div className="border-b border-[#2c3a24] px-3 py-2 text-sm font-semibold text-[#e8d9a0]">
        Летопись боя
      </div>

      <div className="flex min-h-0 flex-1 flex-col-reverse gap-1 overflow-y-auto px-3 py-2 text-xs text-[#c9c9b0]">
        {logs.map((entry) => (
          <LogLine key={entry.id} entry={entry} />
        ))}
      </div>
    </div>
  )
}

function LogLine({ entry }: { entry: BattleLogEntry }) {
  const actor = <span className="text-[#e8d9a0]">{entry.actorName}</span>

  switch (entry.entryType) {
    case 'RoundStartedLogEntryDto':
      return (
        <div className="my-1 flex items-center gap-2 text-[#9c7b3a]">
          <span className="h-px flex-1 bg-[#3a4a30]" />
          <span>Раунд {entry.roundNumber}</span>
          <span className="h-px flex-1 bg-[#3a4a30]" />
        </div>
      )

    case 'AbilityUsedLogEntryDto': {
      const isHeal = entry.effectType === 2

      return (
        <div>
          {actor} — <span className="text-[#d9c07a]">{entry.abilityName}</span> →{' '}
          {entry.targetName}:{' '}
          <span className={isHeal ? 'text-[#9fe06a]' : 'text-[#ff8a6a]'}>
            {isHeal ? '+' : '−'}
            {Math.round(entry.value)}
          </span>
        </div>
      )
    }

    case 'WalkLogEntryDto':
      return (
        <div className="text-[#a8a88f]">
          {actor} идёт к ({entry.to.x}, {entry.to.y})
        </div>
      )

    case 'DeathLogEntryDto':
      return <div className="text-[#c2543f]">{entry.actorName} пал в бою ⚰</div>

    case 'StatusAppliedLogEntryDto': {
      const meta = statusMeta(entry.statusType)

      return (
        <div>
          {actor} → {entry.targetName}:{' '}
          <span className={meta.kind === 'good' ? 'text-[#9fe06a]' : 'text-[#ff8a6a]'}>
            {meta.name}
          </span>{' '}
          ({String(entry.duration)} х., сила {String(entry.magnitude)})
        </div>
      )
    }

    case 'ItemPickedUpLogEntryDto':
      return (
        <div className="text-[#9fe06a]">
          {actor} подобрал {entry.itemName} (+
          {Math.round(entry.value)} ❤)
        </div>
      )

    default:
      return (
        <div className={cn('text-[#a8a88f] italic')}>
          {actor}: {entry.info ?? '…'}
        </div>
      )
  }
}
