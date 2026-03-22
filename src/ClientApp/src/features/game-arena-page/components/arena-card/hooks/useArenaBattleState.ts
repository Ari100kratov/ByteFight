import { useShallow } from "zustand/react/shallow"
import { useGameRuntimeStore } from "@/features/game/state/game.runtime.store"
import { isGameSessionActive } from "@/features/game/types/GameSession"

export function useArenaBattleState() {
  return useGameRuntimeStore(
    useShallow((state) => {
      const session = state.session
      const hasSession = Boolean(session)

      const isPlaybackRunning = state.isProcessing || state.queue.length > 0
      const isSessionActive = isGameSessionActive(session)

      const isFinishingPlayback =
        hasSession &&
        !isSessionActive &&
        isPlaybackRunning

      const isBattleBusy = isSessionActive || isFinishingPlayback

      return {
        hasSession,
        isBattleBusy,
      }
    })
  )
}