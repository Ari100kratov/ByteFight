import { useMemo } from "react"
import { useShallow } from "zustand/react/shallow"
import { useGameRuntimeStore } from "@/features/game/state/game.runtime.store"
import { isGameSessionActive } from "@/features/game/types/GameSession"
import { useGameBootstrapStore } from "@/features/game/state/game.bootstrap.store"

export function useCharacterSelectionState() {
  const { sessionCharacterId, isBattleBusy } = useGameRuntimeStore(
    useShallow((state) => {
      const session = state.session
      const isPlaybackRunning = state.isProcessing || state.queue.length > 0
      const isSessionActive = isGameSessionActive(session)

      const isFinishingPlayback =
        Boolean(session) &&
        !isSessionActive &&
        isPlaybackRunning

      return {
        sessionCharacterId: session?.characterId,
        isBattleBusy: isSessionActive || isFinishingPlayback,
      }
    })
  )

  const isBootstrapLoading = useGameBootstrapStore(s => s.isLoading)

  return useMemo(
    () => ({
      sessionCharacterId,
      isCharacterSelectionDisabled: isBattleBusy || isBootstrapLoading,
    }),
    [sessionCharacterId, isBattleBusy, isBootstrapLoading]
  )
}