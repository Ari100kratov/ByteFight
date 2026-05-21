import { Link } from "react-router-dom"

import { cn } from "@/shared/lib/utils"

import type { ChronicleNominationEntry } from "../types"

function formatNominationValue(entry: ChronicleNominationEntry, metricUnit: string) {
  const formatter = new Intl.NumberFormat("ru-RU", {
    maximumFractionDigits: Number.isInteger(entry.value) ? 0 : 1,
  })

  return `${formatter.format(entry.value)} ${metricUnit}`
}

function formatDate(value?: string | null) {
  if (!value) {
    return "без даты"
  }

  return new Date(value).toLocaleDateString("ru-RU", {
    day: "2-digit",
    month: "short",
  })
}

function formatUserName(entry: ChronicleNominationEntry) {
  return [entry.userFirstName, entry.userLastName].filter(Boolean).join(" ")
}

function formatCharacterBuild(entry: ChronicleNominationEntry) {
  return [entry.characterClassName, entry.characterSpecName].filter(Boolean).join(" · ")
}

interface NominationEntryProps {
  entry: ChronicleNominationEntry
  metricUnit: string
  canOpenCharacter: boolean
  compact?: boolean
}

export function NominationEntry({
  entry,
  metricUnit,
  canOpenCharacter,
  compact = false,
}: NominationEntryProps) {
  const userName = formatUserName(entry)
  const characterBuild = formatCharacterBuild(entry)
  const className = cn(
    "group flex min-w-0 items-stretch gap-3 rounded-2xl border bg-background/70 p-3 transition-all",
    canOpenCharacter && "hover:-translate-y-0.5 hover:border-foreground/20 hover:shadow-sm",
    compact && "rounded-xl px-3 py-2",
  )
  const content = (
    <>
      <div
        className={cn(
          "flex size-9 shrink-0 items-center justify-center rounded-full text-sm font-black",
          entry.rank === 1 && "bg-amber-400 text-amber-950",
          entry.rank === 2 && "bg-zinc-300 text-zinc-950",
          entry.rank === 3 && "bg-orange-300 text-orange-950",
          entry.rank > 3 && "bg-muted text-muted-foreground",
        )}
      >
        {entry.rank}
      </div>

      <div className="min-w-0 flex-1">
        <div className="flex min-w-0 items-start justify-between gap-3">
          <div className="min-w-0">
            <div
              className={cn(
                "truncate text-sm font-semibold",
                canOpenCharacter && "group-hover:underline",
              )}
            >
              {entry.characterName}
            </div>
            <div className="mt-1 flex flex-wrap items-center gap-1.5">
              <span className="bg-muted text-muted-foreground max-w-[13rem] truncate rounded-full px-2 py-0.5 text-[11px] font-medium">
                {characterBuild || "Класс не указан"}
              </span>
            </div>
          </div>

          <div className="shrink-0 text-right">
            <div className="text-sm font-bold">{formatNominationValue(entry, metricUnit)}</div>
          </div>
        </div>

        <div className="text-muted-foreground mt-2 flex flex-wrap items-center gap-2 text-[11px]">
          <span className="bg-background/70 max-w-[14rem] truncate rounded-full border px-2 py-0.5">
            Игрок: {userName || "неизвестен"}
          </span>
          <span>{formatDate(entry.occurredAtUtc)}</span>
        </div>
      </div>
    </>
  )

  return canOpenCharacter ? (
    <Link to={`/characters/${entry.characterId}`} className={className}>
      {content}
    </Link>
  ) : (
    <div className={className}>{content}</div>
  )
}
