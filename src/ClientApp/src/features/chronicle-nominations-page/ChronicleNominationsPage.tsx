import { useQuery } from "@tanstack/react-query"
import { Trophy } from "lucide-react"

import { LoaderState } from "@/components/common/LoaderState"
import { queryKeys } from "@/shared/lib/queryKeys"
import { useCurrentUser } from "@/features/nav-user-menu-item/hooks/useCurrentUser"

import { getChronicleLeaderboards } from "./api/getChronicleLeaderboards"
import { ChronicleNominationsSkeleton } from "./components/ChronicleNominationsSkeleton"
import { NominationCard } from "./components/NominationCard"

const TOP_LIMIT = 4

export default function ChronicleNominationsPage() {
  const { data, isLoading, error } = useQuery({
    queryKey: queryKeys.chronicles.leaderboards(TOP_LIMIT),
    queryFn: () => getChronicleLeaderboards({ top: TOP_LIMIT }),
  })
  const { data: currentUser } = useCurrentUser()
  const canOpenCharacters =
    currentUser?.roles?.some((role) => role.toLowerCase() === "admin") ?? false
  const visibleNominations = (data ?? []).filter((nomination) => nomination.entries.length > 0)

  return (
    <div className="min-h-full w-full">
      <LoaderState
        isLoading={isLoading}
        error={error}
        loadingFallback={<ChronicleNominationsSkeleton />}
      >
        <div className="flex flex-col gap-4 p-2 md:p-4">
          <section className="relative overflow-hidden rounded-3xl border bg-[radial-gradient(circle_at_top_left,oklch(0.88_0.16_84/.28),transparent_34%),linear-gradient(135deg,oklch(0.17_0.03_245),oklch(0.1_0.02_250))] p-6 text-white shadow-sm md:p-8">
            <div className="absolute top-6 right-6 hidden rounded-full border border-white/15 px-4 py-2 text-sm text-white/70 md:block">
              Хроники считают, но не осуждают
            </div>
            <div className="max-w-2xl">
              <div className="mb-4 inline-flex items-center gap-2 rounded-full border border-white/15 bg-white/10 px-3 py-1 text-sm text-white/80">
                <Trophy className="size-4" />
                Номинации арены
              </div>
              <h1 className="text-3xl font-black tracking-tight md:text-5xl">Зал славы</h1>
              <p className="mt-4 max-w-xl text-sm leading-6 text-white/70 md:text-base">
                Арена помнит не только победителей. Здесь собираются те, кто оставил след в её
                хрониках — силой, упрямством или полным безумием своих решений.
              </p>
            </div>
          </section>

          {visibleNominations.length > 0 ? (
            <div className="grid gap-4 lg:grid-cols-2 xl:grid-cols-3">
              {visibleNominations.map((nomination) => (
                <NominationCard
                  key={nomination.code}
                  nomination={nomination}
                  canOpenCharacters={canOpenCharacters}
                />
              ))}
            </div>
          ) : (
            <div className="bg-muted/30 text-muted-foreground rounded-3xl border border-dashed p-8 text-sm">
              Зал славы пока пуст. Он заполнится после обработки завершенных боев.
            </div>
          )}
        </div>
      </LoaderState>
    </div>
  )
}
