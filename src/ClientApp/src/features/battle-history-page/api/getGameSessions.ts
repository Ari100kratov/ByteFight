import { apiFetch } from "@/shared/lib/apiFetch"
import type { GameSessionListItem } from "../types"
import type { PagedResponse } from "../../../shared/types/pagedResponse"

type GetGameSessionsParams = {
  page: number
  pageSize: number
}

export async function getGameSessions({
  page,
  pageSize,
}: GetGameSessionsParams): Promise<PagedResponse<GameSessionListItem>> {
  const search = new URLSearchParams({
    page: String(page),
    pageSize: String(pageSize),
  })

  return await apiFetch<PagedResponse<GameSessionListItem>>(
    `/game/sessions?${search.toString()}`
  )
}