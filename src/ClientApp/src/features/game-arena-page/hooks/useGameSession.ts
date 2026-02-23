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

  const { data: session, isLoading } = useQuery<GameSession, ApiException>({
    queryKey: queryKeys.gameSessions.byId(sessionId),
    queryFn: () => apiFetch(`/game/sessions/${sessionId}`),
    enabled: !!sessionId,
    retry: false,
  })

  const { data: logs } = useQuery<TurnLog[], ApiException>({
    queryKey: queryKeys.gameSessions.logs(sessionId),
    queryFn: () => apiFetch(`/game/sessions/${sessionId}/logs`),
    enabled: !!sessionId,
    retry: false,
  });


  useEffect(() => {
    if (isLoading) useGameBootstrapStore.getState().start()
  }, [isLoading])

  useEffect(() => {
    return () => {
      resetGameStores()
    }
  }, [])

  useEffect(() => {
    if (!session) return
    setSession(session)
  }, [session])

  useEffect(() => {
    if (!logs?.length) return
    setTurnLogs(logs)
  }, [logs])

  useEffect(() => {
    if (!session || !isGameSessionActive(session)) {
      useGameBootstrapStore.getState().end()
      return
    }

    gameHub.connect(session.id)
      .catch(err => {
        console.error("Failed to connect to game hub:", err)
      })
      .finally(() => {
        useGameBootstrapStore.getState().end()
      })

    return () => {
      gameHub.disconnect(session.id).catch(() => { })
    }
  }, [session?.id])
}
