import { useMemo, useState } from "react"
import { useNavigate } from "react-router-dom"
import { useQuery } from "@tanstack/react-query"
import { Skeleton } from "@/components/ui/skeleton"
import { LoaderState } from "@/components/common/LoaderState"

import { getGameSessions } from "./api/getGameSessions"
import { createBattleHistoryColumns } from "./components/battleHistory.columns"
import { BattleHistoryTable } from "./components/BattleHistoryTable"
import { BattleHistoryPagination } from "./components/BattleHistoryPagination"
import type { GameSessionListItem } from "./types"
import { formatModeSlugByType } from "@/shared/types/modeNames"

const PAGE_SIZE = 15

function BattleHistoryPageSkeleton() {
  return (
    <div className="flex flex-col gap-4 w-full h-full p-4 md:p-6">
      <div className="overflow-hidden rounded-xl border bg-background">
        <div className="grid grid-cols-6 gap-4 border-b px-4 py-3">
          <Skeleton className="h-4 w-24" />
          <Skeleton className="h-4 w-24" />
          <Skeleton className="h-4 w-28" />
          <Skeleton className="h-4 w-20" />
          <Skeleton className="h-4 w-24" />
          <Skeleton className="h-4 w-16" />
        </div>

        <div className="divide-y">
          {Array.from({ length: 8 }).map((_, index) => (
            <div
              key={index}
              className="grid grid-cols-6 gap-4 px-4 py-4 items-center"
            >
              <Skeleton className="h-4 w-28" />
              <Skeleton className="h-4 w-20" />
              <Skeleton className="h-4 w-36" />
              <Skeleton className="h-4 w-16" />
              <Skeleton className="h-4 w-24" />
              <Skeleton className="h-8 w-20 rounded-md" />
            </div>
          ))}
        </div>
      </div>

      <div className="flex items-center justify-between gap-4">
        <Skeleton className="h-4 w-40" />
        <div className="flex items-center gap-2">
          <Skeleton className="h-9 w-24 rounded-md" />
          <Skeleton className="h-9 w-24 rounded-md" />
        </div>
      </div>
    </div>
  )
}

export default function BattleHistoryPage() {
  const [page, setPage] = useState(1)
  const navigate = useNavigate()

  const { data, isLoading, error, isFetching } = useQuery({
    queryKey: ["game-sessions", page, PAGE_SIZE],
    queryFn: () => getGameSessions({ page, pageSize: PAGE_SIZE }),
    placeholderData: previous => previous,
  })

  const columns = useMemo(() => createBattleHistoryColumns(), [])

  const handleRowClick = (session: GameSessionListItem) => {
    navigate(`/play/${formatModeSlugByType(session.mode)}/${session.arenaId}/${session.id}`)
  }

  return (
    <div className="flex h-full min-h-0 w-full flex-col">
      <LoaderState
        isLoading={isLoading}
        error={error}
        skeletonClassName="w-full h-full"
        loadingFallback={<BattleHistoryPageSkeleton />}
      >
        <div className="flex h-full min-h-0 flex-col gap-2 p-2">
          <div className="min-h-0 flex-1">
            <BattleHistoryTable
              columns={columns}
              data={data?.items ?? []}
              onRowClick={handleRowClick}
            />
          </div>

          <BattleHistoryPagination
            page={data?.page ?? page}
            pageSize={data?.pageSize ?? PAGE_SIZE}
            totalCount={data?.totalCount ?? 0}
            isFetching={isFetching}
            onPageChange={setPage}
          />
        </div>
      </LoaderState>
    </div>
  )
}