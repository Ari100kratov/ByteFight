import { apiFetch } from "@/shared/lib/apiFetch"
import type { ChronicleNominationLeaderboard } from "../types"

type GetChronicleLeaderboardsParams = {
  top: number
}

export async function getChronicleLeaderboards({
  top,
}: GetChronicleLeaderboardsParams): Promise<ChronicleNominationLeaderboard[]> {
  const search = new URLSearchParams({
    top: String(top),
  })

  return await apiFetch<ChronicleNominationLeaderboard[]>(
    `/chronicles/leaderboards?${search.toString()}`
  )
}
