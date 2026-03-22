import { useEffect, useMemo } from "react"
import { useGameRuntimeStore } from "@/features/game/state/game.runtime.store"
import { isGameSessionActive } from "@/features/game/types/GameSession"
import { useBattleResultUiStore } from "../../battle-result-overlay/hooks/useBattleResultUiStore"
import { getBattleResultMeta } from "../../battle-result-overlay/helpers/getBattleResultMeta"
import { useArenaBattleState } from "./useArenaBattleState"

export function useArenaBattleResult() {
  const session = useGameRuntimeStore(s => s.session)
  const { isBattleBusy } = useArenaBattleState()

  const isOpen = useBattleResultUiStore(s => s.isOpen)
  const showForSession = useBattleResultUiStore(s => s.showForSession)
  const open = useBattleResultUiStore(s => s.open)
  const close = useBattleResultUiStore(s => s.close)

  const hasResult = Boolean(session?.result)
  const isSessionFinished = Boolean(session) && !isGameSessionActive(session)

  const canShowResult =
    Boolean(session?.id) &&
    hasResult &&
    isSessionFinished &&
    !isBattleBusy

  useEffect(() => {
    if (!session?.id || !canShowResult) {
      return
    }

    showForSession(session.id)
  }, [session?.id, canShowResult, showForSession])

  const resultMeta = useMemo(() => {
  return hasResult
    ? getBattleResultMeta(session?.result?.outcome, session?.id)
    : null
}, [hasResult, session?.result?.outcome, session?.id])

  const resultView = useMemo(() => {
    if (!session?.result || !resultMeta) {
      return null
    }

    return {
      title: resultMeta.title,
      description: resultMeta.description,
      tone: resultMeta.tone,
      Icon: resultMeta.icon,
      totalTurns: session.totalTurns ?? null,
      startedAt: session.startedAt,
      endedAt: session.endedAt,
      outcome: session.result.outcome,
    }
  }, [session, resultMeta])

  return {
    session,
    resultView,
    canShowResult,
    isResultOpen: isOpen,
    openResult: open,
    closeResult: close,
  }
}