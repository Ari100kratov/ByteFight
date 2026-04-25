import type { ColumnDef } from "@tanstack/react-table"
import type { GameSessionListItem } from "../types"
import { formatBattleDuration } from "@/features/game-arena-page/components/battle-result-overlay/helpers/formatBattleDuration"
import { formatModeNameByType } from "@/shared/types/modeNames"
import { mapBattleHistoryResult } from "../helpers/mapBattleHistoryResult"
import { BattleHistoryResultBadge } from "./BattleHistoryResultBadge"
import { CharacterIdentity } from "@/features/characters/components/CharacterIdentity"

function formatDate(value: string) {
  return new Date(value).toLocaleString("ru-RU", {
    day: "2-digit",
    month: "2-digit",
    year: "numeric",
    hour: "2-digit",
    minute: "2-digit",
  })
}

function CharacterCell({
  name,
  className,
  specName,
}: {
  name?: string | null
  className?: string | null
  specName?: string | null
}) {
  return (
    <CharacterIdentity
      name={name}
      className={className}
      specName={specName}
      size="sm"
    />
  )
}

function ArenaCell({ arenaName }: { arenaName?: string | null }) {
  return (
    <div className="min-w-0">
      <div className="line-clamp-2">
        {arenaName ?? "—"}
      </div>
    </div>
  )
}

export function createBattleHistoryColumns(): ColumnDef<GameSessionListItem>[] {
  return [
    {
      id: "result",
      cell: ({ row }) => {
        const result = mapBattleHistoryResult({
          status: row.original.status,
          outcome: row.original.outcome,
        })

        return (
          <BattleHistoryResultBadge
            title={result.title}
            tone={result.tone}
            Icon={result.Icon}
          />
        )
      },
    },
    {
      id: "character",
      header: "Персонаж",
      cell: ({ row }) => (
        <CharacterCell
          name={row.original.characterName}
          className={row.original.characterClassName}
          specName={row.original.characterSpecName}
        />
      ),
    },
    {
      accessorKey: "startedAt",
      header: "Дата",
      cell: ({ row }) => formatDate(row.original.startedAt),
    },
    {
      accessorKey: "mode",
      header: "Режим",
      cell: ({ row }) => formatModeNameByType(row.original.mode),
    },
    {
      accessorKey: "arenaName",
      header: "Арена",
      cell: ({ row }) => (
        <ArenaCell arenaName={row.original.arenaName} />
      ),
    },
    {
      accessorKey: "totalTurns",
      header: "Ходы",
      cell: ({ row }) => row.original.totalTurns ?? "—",
    },
    {
      id: "duration",
      header: "Длительность",
      cell: ({ row }) =>
        formatBattleDuration(row.original.startedAt, row.original.endedAt),
    },
  ]
}