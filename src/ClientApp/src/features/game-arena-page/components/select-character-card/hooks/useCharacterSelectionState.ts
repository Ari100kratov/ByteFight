import { useMemo } from "react"
import { useShallow } from "zustand/react/shallow"
import { useGameRuntimeStore } from "@/features/game/state/game.runtime.store"
import { useGameBootstrapStore } from "@/features/game/state/game.bootstrap.store"

export function useCharacterSelectionState() {
  const { sessionCharacterId, hasSession } = useGameRuntimeStore(
    useShallow((state) => {
      const session = state.session

      return {
        sessionCharacterId: session?.characterId,
        hasSession: Boolean(session),
      }
    })
  )

  const isBootstrapLoading = useGameBootstrapStore((s) => s.isLoading)

  return useMemo(
    () => ({
      sessionCharacterId,
      isCharacterSelectionDisabled: hasSession || isBootstrapLoading,
    }),
    [sessionCharacterId, hasSession, isBootstrapLoading]
  )
}