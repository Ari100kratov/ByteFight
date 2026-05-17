export type ChronicleNominationValueKind = "turns" | "count" | "damage" | "healing"

export type ChronicleNominationSortDirection = "ascending" | "descending"

export interface ChronicleNominationEntry {
  rank: number
  characterId: string
  characterName: string
  userFirstName?: string | null
  userLastName?: string | null
  characterClassName?: string | null
  characterSpecName?: string | null
  value: number
  sessionId?: string | null
  occurredAtUtc?: string | null
}

export interface ChronicleNominationLeaderboard {
  code: string
  title: string
  description: string
  metricLabel: string
  metricUnit: string
  valueKind: ChronicleNominationValueKind
  sortDirection: ChronicleNominationSortDirection
  entries: ChronicleNominationEntry[]
}
