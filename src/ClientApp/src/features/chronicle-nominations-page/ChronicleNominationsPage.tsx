import { Link } from "react-router-dom"
import { useQuery } from "@tanstack/react-query"
import {
  Activity,
  Ambulance,
  BadgeAlert,
  Dumbbell,
  Footprints,
  Handshake,
  HeartPulse,
  PackageOpen,
  Shield,
  Sparkles,
  Swords,
  Trophy,
  type LucideIcon,
} from "lucide-react"

import { LoaderState } from "@/components/common/LoaderState"
import { Badge } from "@/components/ui/badge"
import {
  Card,
  CardContent,
  CardDescription,
  CardHeader,
  CardTitle,
} from "@/components/ui/card"
import { Skeleton } from "@/components/ui/skeleton"
import { cn } from "@/shared/lib/utils"
import { queryKeys } from "@/shared/lib/queryKeys"
import { useCurrentUser } from "@/features/nav-user-menu-item/hooks/useCurrentUser"

import { getChronicleLeaderboards } from "./api/getChronicleLeaderboards"
import type {
  ChronicleNominationEntry,
  ChronicleNominationLeaderboard,
} from "./types"

const TOP_LIMIT = 4

type NominationVisual = {
  Icon: LucideIcon
  accent: string
  glow: string
}

const nominationVisuals: Record<string, NominationVisual> = {
  speedrun_without_tutorial: {
    Icon: Trophy,
    accent: "from-amber-500/25 via-orange-400/15 to-transparent",
    glow: "bg-amber-500/10 text-amber-700 dark:text-amber-300",
  },
  battle_telenovela: {
    Icon: Activity,
    accent: "from-rose-500/20 via-red-400/10 to-transparent",
    glow: "bg-rose-500/10 text-rose-700 dark:text-rose-300",
  },
  fight_subscription: {
    Icon: Swords,
    accent: "from-sky-500/20 via-cyan-400/10 to-transparent",
    glow: "bg-sky-500/10 text-sky-700 dark:text-sky-300",
  },
  scheduled_winner: {
    Icon: Sparkles,
    accent: "from-yellow-500/25 via-lime-400/10 to-transparent",
    glow: "bg-yellow-500/10 text-yellow-700 dark:text-yellow-300",
  },
  useful_experience_master: {
    Icon: BadgeAlert,
    accent: "from-stone-500/18 via-zinc-400/10 to-transparent",
    glow: "bg-stone-500/10 text-stone-700 dark:text-stone-300",
  },
  fist_diplomat: {
    Icon: Handshake,
    accent: "from-teal-500/20 via-emerald-400/10 to-transparent",
    glow: "bg-teal-500/10 text-teal-700 dark:text-teal-300",
  },
  unlicensed_surgeon: {
    Icon: Dumbbell,
    accent: "from-red-500/20 via-orange-500/10 to-transparent",
    glow: "bg-red-500/10 text-red-700 dark:text-red-300",
  },
  budget_ambulance: {
    Icon: Ambulance,
    accent: "from-emerald-500/22 via-green-400/10 to-transparent",
    glow: "bg-emerald-500/10 text-emerald-700 dark:text-emerald-300",
  },
  armored_cardio: {
    Icon: Footprints,
    accent: "from-indigo-500/20 via-blue-400/10 to-transparent",
    glow: "bg-indigo-500/10 text-indigo-700 dark:text-indigo-300",
  },
  arena_vacuum_cleaner: {
    Icon: PackageOpen,
    accent: "from-fuchsia-500/20 via-pink-400/10 to-transparent",
    glow: "bg-fuchsia-500/10 text-fuchsia-700 dark:text-fuchsia-300",
  },
  strategic_idle: {
    Icon: Shield,
    accent: "from-slate-500/20 via-gray-400/10 to-transparent",
    glow: "bg-slate-500/10 text-slate-700 dark:text-slate-300",
  },
  damage_sponge: {
    Icon: HeartPulse,
    accent: "from-cyan-500/20 via-blue-300/10 to-transparent",
    glow: "bg-cyan-500/10 text-cyan-700 dark:text-cyan-300",
  },
}

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
  return [entry.userFirstName, entry.userLastName]
    .filter(Boolean)
    .join(" ")
}

function formatCharacterBuild(entry: ChronicleNominationEntry) {
  return [entry.characterClassName, entry.characterSpecName]
    .filter(Boolean)
    .join(" · ")
}

function getVisual(code: string): NominationVisual {
  return nominationVisuals[code] ?? {
    Icon: Trophy,
    accent: "from-primary/15 via-muted to-transparent",
    glow: "bg-muted text-foreground",
  }
}

function PodiumEntry({
  entry,
  metricUnit,
  canOpenCharacter,
  compact = false,
}: {
  entry: ChronicleNominationEntry
  metricUnit: string
  canOpenCharacter: boolean
  compact?: boolean
}) {
  const userName = formatUserName(entry)
  const characterBuild = formatCharacterBuild(entry)
  const className = cn(
    "group flex min-w-0 items-stretch gap-3 rounded-2xl border bg-background/70 p-3 transition-all",
    canOpenCharacter && "hover:-translate-y-0.5 hover:border-foreground/20 hover:shadow-sm",
    compact && "rounded-xl px-3 py-2"
  )
  const content = (
    <>
      <div
        className={cn(
          "flex size-9 shrink-0 items-center justify-center rounded-full text-sm font-black",
          entry.rank === 1 && "bg-amber-400 text-amber-950",
          entry.rank === 2 && "bg-zinc-300 text-zinc-950",
          entry.rank === 3 && "bg-orange-300 text-orange-950",
          entry.rank > 3 && "bg-muted text-muted-foreground"
        )}
      >
        {entry.rank}
      </div>

      <div className="min-w-0 flex-1">
        <div className="flex min-w-0 items-start justify-between gap-3">
          <div className="min-w-0">
            <div className={cn("truncate text-sm font-semibold", canOpenCharacter && "group-hover:underline")}>
              {entry.characterName}
            </div>
            <div className="mt-1 flex flex-wrap items-center gap-1.5">
              <span className="max-w-[13rem] truncate rounded-full bg-muted px-2 py-0.5 text-[11px] font-medium text-muted-foreground">
                {characterBuild || "Класс не указан"}
              </span>
            </div>
          </div>

          <div className="shrink-0 text-right">
            <div className="text-sm font-bold">
              {formatNominationValue(entry, metricUnit)}
            </div>
          </div>
        </div>

        <div className="mt-2 flex flex-wrap items-center gap-2 text-[11px] text-muted-foreground">
          <span className="max-w-[14rem] truncate rounded-full border bg-background/70 px-2 py-0.5">
            Игрок: {userName || "неизвестен"}
          </span>
          <span>{formatDate(entry.occurredAtUtc)}</span>
        </div>
      </div>
    </>
  )

  return canOpenCharacter ? (
    <Link
      to={`/characters/${entry.characterId}`}
      className={className}
    >
      {content}
    </Link>
  ) : (
    <div className={className}>
      {content}
    </div>
  )
}

function NominationCard({
  nomination,
  canOpenCharacters,
}: {
  nomination: ChronicleNominationLeaderboard
  canOpenCharacters: boolean
}) {
  const visual = getVisual(nomination.code)
  const podium = nomination.entries.slice(0, 3)
  const rest = nomination.entries.slice(3)

  return (
    <Card className="relative overflow-hidden border-foreground/10 bg-card/95">
      <div className={cn("absolute inset-x-0 top-0 h-32 bg-gradient-to-b", visual.accent)} />
      <CardHeader className="relative">
        <div className="flex items-start gap-4">
          <div className={cn("flex size-12 shrink-0 items-center justify-center rounded-2xl", visual.glow)}>
            <visual.Icon className="size-6" />
          </div>
          <div className="min-w-0">
            <CardTitle className="leading-tight">{nomination.title}</CardTitle>
            <CardDescription className="mt-2 line-clamp-2">
              {nomination.description}
            </CardDescription>
          </div>
        </div>

        <div className="mt-4 flex flex-wrap gap-2">
          <Badge variant="secondary">{nomination.metricLabel}</Badge>
          <Badge variant="outline">
            {nomination.sortDirection === "ascending" ? "рекорд на минимум" : "рекорд на максимум"}
          </Badge>
        </div>
      </CardHeader>

      <CardContent className="relative space-y-3">
        <div className="grid gap-2">
          {podium.map(entry => (
            <PodiumEntry
              key={entry.characterId}
              entry={entry}
              metricUnit={nomination.metricUnit}
              canOpenCharacter={canOpenCharacters}
            />
          ))}
        </div>

        {rest.length > 0 && (
          <div className="grid gap-2 border-t pt-3">
            {rest.map(entry => (
              <PodiumEntry
                key={entry.characterId}
                entry={entry}
                metricUnit={nomination.metricUnit}
                canOpenCharacter={canOpenCharacters}
                compact
              />
            ))}
          </div>
        )}
      </CardContent>
    </Card>
  )
}

function ChronicleNominationsSkeleton() {
  return (
    <div className="flex flex-col gap-4 p-2 md:p-4">
      <Skeleton className="h-40 rounded-3xl" />
      <div className="grid gap-4 lg:grid-cols-2 xl:grid-cols-3">
        {Array.from({ length: 6 }).map((_, index) => (
          <Skeleton key={index} className="h-80 rounded-xl" />
        ))}
      </div>
    </div>
  )
}

export default function ChronicleNominationsPage() {
  const { data, isLoading, error } = useQuery({
    queryKey: queryKeys.chronicles.leaderboards(TOP_LIMIT),
    queryFn: () => getChronicleLeaderboards({ top: TOP_LIMIT }),
  })
  const { data: currentUser } = useCurrentUser()
  const canOpenCharacters = currentUser?.roles?.some(role => role.toLowerCase() === "admin") ?? false
  const visibleNominations = (data ?? []).filter(nomination => nomination.entries.length > 0)

  return (
    <div className="min-h-full w-full">
      <LoaderState
        isLoading={isLoading}
        error={error}
        loadingFallback={<ChronicleNominationsSkeleton />}
      >
        <div className="flex flex-col gap-4 p-2 md:p-4">
          <section className="relative overflow-hidden rounded-3xl border bg-[radial-gradient(circle_at_top_left,oklch(0.88_0.16_84/.28),transparent_34%),linear-gradient(135deg,oklch(0.17_0.03_245),oklch(0.1_0.02_250))] p-6 text-white shadow-sm md:p-8">
            <div className="absolute right-6 top-6 hidden rounded-full border border-white/15 px-4 py-2 text-sm text-white/70 md:block">
              хроники считают, но не осуждают
            </div>
            <div className="max-w-2xl">
              <div className="mb-4 inline-flex items-center gap-2 rounded-full border border-white/15 bg-white/10 px-3 py-1 text-sm text-white/80">
                <Trophy className="size-4" />
                Номинации арены
              </div>
              <h1 className="text-3xl font-black tracking-tight md:text-5xl">
                Зал славы
              </h1>
              <p className="mt-4 max-w-xl text-sm leading-6 text-white/70 md:text-base">
                Витрина самых заметных достижений арены: кто закрывает бои
                быстрее всех, вытаскивает затяжные схватки, спасает союзников
                и превращает странные решения в легендарные.
              </p>
            </div>
          </section>

          {visibleNominations.length > 0 ? (
            <div className="grid gap-4 lg:grid-cols-2 xl:grid-cols-3">
              {visibleNominations.map(nomination => (
                <NominationCard
                  key={nomination.code}
                  nomination={nomination}
                  canOpenCharacters={canOpenCharacters}
                />
              ))}
            </div>
          ) : (
            <div className="rounded-3xl border border-dashed bg-muted/30 p-8 text-sm text-muted-foreground">
              Зал славы пока пуст. Он заполнится после обработки завершенных боев.
            </div>
          )}
        </div>
      </LoaderState>
    </div>
  )
}
