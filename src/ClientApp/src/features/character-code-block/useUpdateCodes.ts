import { useMutation, useQueryClient } from "@tanstack/react-query"
import { apiFetch } from "@/lib/apiFetch"

export interface UpdateCodesRequest {
  characterId: string
  created: { id: string; name: string; sourceCode?: string | null }[]
  updated: { id: string; name: string; sourceCode?: string | null }[]
  deletedIds: string[]
}

export function useUpdateCodes() {
  const queryClient = useQueryClient()

  return useMutation({
    mutationFn: async (data: UpdateCodesRequest) => {
      return apiFetch(`/characters/${data.characterId}/codes`, {
        method: "POST",
        body: JSON.stringify({
          created: data.created,
          updated: data.updated,
          deletedIds: data.deletedIds,
        }),
      })
    },
    onSuccess: (_, variables) => {
      queryClient.invalidateQueries({ queryKey: ["characterCodes", variables.characterId] })
    },
  })
}
