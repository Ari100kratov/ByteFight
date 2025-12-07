import { useMutation } from '@tanstack/react-query'
import { apiFetch } from '@/shared/lib/apiFetch'

export type StartGameRequest = {
  arenaId: string
  mode: string
  characterId: string
  code?: string
}

export type StartGameResponse = {
  id: string // gameSessionId
}

export function useStartGame() {
  return useMutation({
    mutationFn: async (data: StartGameRequest): Promise<string> => {
      if (!data.arenaId) throw new Error("Не указан ArenaId")
      if (!data.characterId) throw new Error("Не выбран персонаж")
      if (!data.mode) throw new Error("Не указан режим игры")

      const response = await apiFetch<StartGameResponse>("/game/start", {
        method: "POST",
        body: JSON.stringify(data)
      })

      return response.id
    },
  })
}
