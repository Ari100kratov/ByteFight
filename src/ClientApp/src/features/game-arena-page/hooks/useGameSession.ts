import { useEffect } from "react"
import { gameHub } from "@/features/game/api/gameHub"
import { useGameRuntimeStore } from "@/features/game/state/game.runtime.store"
import { resetGameStores } from "@/features/game/state/stateReset"
import { useQuery } from "@tanstack/react-query"
import { isGameSessionActive, type GameSession } from "@/features/game/types/GameSession"
import { apiFetch, type ApiException } from "@/shared/lib/apiFetch"
import { queryKeys } from "@/shared/lib/queryKeys"
import { useGameBootstrapStore } from "@/features/game/state/game.bootstrap.store"
import type { TurnLog } from "@/features/game/types/TurnLog"

export function useGameSession(sessionId?: string) {
  const { setSession, setTurnLogs } = useGameRuntimeStore()

  const hasSessionId = !!sessionId

  const { data: session, isLoading } = useQuery<GameSession, ApiException>({
    queryKey: queryKeys.gameSessions.byId(sessionId),
    queryFn: () => {
      if (!sessionId) {
        throw new Error("Game session id is required")
      }

      return apiFetch(`/game/sessions/${sessionId}`)
    },
    enabled: hasSessionId,
    retry: false,
  })

  const { data: logs } = useQuery<TurnLog[], ApiException>({
    queryKey: queryKeys.gameSessions.logs(sessionId),
    queryFn: () => {
      if (!sessionId) {
        throw new Error("Game session id is required")
      }

      return apiFetch(`/game/sessions/${sessionId}/logs`)
    },
    enabled: hasSessionId,
    retry: false,
    refetchOnWindowFocus: false,
  })

  useEffect(() => {
    if (isLoading) {
      useGameBootstrapStore.getState().start()
    }
  }, [isLoading])

  useEffect(() => {
    return () => {
      resetGameStores()
    }
  }, [])

  useEffect(() => {
    if (!session) return
    setSession(session)
  }, [session, setSession])

  useEffect(() => {
    setTurnLogs(logs ?? [])
  }, [logs, setTurnLogs])

  const isActive = session ? isGameSessionActive(session) : false

  useEffect(() => {
    if (!session) return

    if (!isActive) {
      useGameBootstrapStore.getState().end()
      return
    }

    gameHub
      .connect(session.id)
      .catch((err: unknown) => {
        console.error("Failed to connect to game hub:", err)
      })
      .finally(() => {
        useGameBootstrapStore.getState().end()
      })

    return () => {
      gameHub.disconnect(session.id).catch((err: unknown) => {
        console.error("Failed to disconnect from game hub:", err)
      })
    }
  }, [isActive, session])
}
