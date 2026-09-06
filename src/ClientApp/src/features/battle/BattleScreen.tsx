import { useEffect } from 'react'
import { useNavigate } from 'react-router-dom'

import { Button } from '@/components/ui/button'

import { battleHub } from './api/battleHub'
import { BattleCanvas } from './render/BattleCanvas'
import { useBattleStore } from './state/battle.store'
import { ActionBar } from './ui/ActionBar'
import { BattleLogPanel } from './ui/BattleLogPanel'
import { TurnQueuePanel, UnitInspector } from './ui/BattleHud'

interface BattleScreenProps {
  sessionId: string
}

/**
 * Экран живого боя: поле, панель действий, очередь ходов,
 * инспектор юнитов и летопись.
 */
export function BattleScreen({ sessionId }: BattleScreenProps) {
  const finished = useBattleStore((s) => s.finished)
  const hasState = useBattleStore((s) => s.state !== null)
  const reset = useBattleStore((s) => s.reset)
  const navigate = useNavigate()

  useEffect(() => {
    void battleHub.connect(sessionId)

    return () => {
      void battleHub.disconnect(sessionId)
      reset()
    }
  }, [sessionId, reset])

  return (
    <div className="flex h-full min-h-0 w-full flex-col gap-3">
      <div className="flex min-h-0 flex-1 gap-3">
        <div className="flex min-w-56 max-w-64 flex-col gap-3">
          <TurnQueuePanel />
          <UnitInspector />
        </div>

        <div className="min-h-0 flex-1">
          {hasState ? (
            <BattleCanvas sessionId={sessionId} />
          ) : (
            <div className="flex h-full items-center justify-center rounded-xl border border-[#2c3a24] bg-[#11180f]">
              <span className="animate-pulse text-[#a8a88f]">Рубка начинается...</span>
            </div>
          )}
        </div>

        <div className="hidden min-h-0 w-72 xl:block">
          <BattleLogPanel />
        </div>
      </div>

      <ActionBar sessionId={sessionId} />

      {finished && (
        <div className="absolute inset-0 z-50 flex items-center justify-center bg-[#11180f]/90">
          <div className="flex flex-col items-center gap-4 rounded-2xl border border-[#9c7b3a] bg-[#161f12] p-8">
            <span className="text-2xl font-semibold text-[#e8d9a0]">Бой окончен</span>
            <span className="text-sm text-[#c9c9b0]">Летопись сражения сохранена в хрониках.</span>
            <Button
              variant="outline"
              className="border-[#9c7b3a] text-[#e8d9a0] hover:bg-[#2c3a24]"
              onClick={() => { void navigate('/play') }}
            >
              Выйти на росстань
            </Button>
          </div>
        </div>
      )}
    </div>
  )
}
