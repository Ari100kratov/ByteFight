import { useEffect, useMemo } from "react"
import { useGameRuntimeStore } from "@/features/game/state/game.runtime.store"
import { GameStatus, isGameSessionActive } from "@/features/game/types/GameSession"
import { useBattleResultUiStore } from "../../battle-result-overlay/hooks/useBattleResultUiStore"
import { mapBattleResultMeta } from "../../battle-result-overlay/helpers/mapBattleResultMeta"
import { useArenaBattleState } from "./useArenaBattleState"

export function useArenaBattleResult() {
  const session = useGameRuntimeStore(s => s.session)
  const { isBattleBusy } = useArenaBattleState()

  const isOpen = useBattleResultUiStore(s => s.isOpen)
  const showForSession = useBattleResultUiStore(s => s.showForSession)
  const open = useBattleResultUiStore(s => s.open)
  const close = useBattleResultUiStore(s => s.close)

  const isSessionFinished = Boolean(session) && !isGameSessionActive(session)

  const hasDisplayableResult =
    session?.status === GameStatus.Completed ||
    session?.status === GameStatus.Aborted ||
    session?.status === GameStatus.Failed

  const canShowResult =
    Boolean(session?.id) &&
    isSessionFinished &&
    hasDisplayableResult &&
    !isBattleBusy

  useEffect(() => {
    if (!session?.id || !canShowResult) {
      return
    }

    showForSession(session.id)
  }, [session?.id, canShowResult, showForSession])

  const resultMeta = useMemo(() => {
    if (!session || !hasDisplayableResult) {
      return null
    }

    return mapBattleResultMeta({
      status: session.status,
      outcome: session.result?.outcome,
      sessionId: session.id,
      errorMessage: session.errorMessage,
    })
  }, [
    session,
    hasDisplayableResult,
  ])

  const resultView = useMemo(() => {
    if (!session || !resultMeta) {
      return null
    }

    return {
      title: resultMeta.title,
      description: resultMeta.description,
      tone: resultMeta.tone,
      Icon: resultMeta.icon,
      totalTurns: session.totalTurns ?? 0,
      startedAt: session.startedAt,
      endedAt: session.endedAt,
      outcome: session.result?.outcome,
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