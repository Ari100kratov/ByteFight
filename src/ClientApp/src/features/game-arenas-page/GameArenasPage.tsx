import { useNavigate, useParams } from "react-router-dom"
import { useArenasByMode, type ArenaResponse } from "./useArenasByMode"
import { LoaderState } from "@/components/common/LoaderState"
import { Skeleton } from "@/components/ui/skeleton"
import { GameArenaCard } from "./components/GameArenaCard"
import { useArenaBreadcrumbs } from "@/shared/hooks/useArenaBreadcrumbs"

export default function GameArenasPage() {
  const navigate = useNavigate()
  const { modeType } = useParams<{ modeType: string }>()

  useArenaBreadcrumbs({ modeType })

  const { data: arenas, isLoading, error } = useArenasByMode(modeType)

  const handleArenaClick = (arena: ArenaResponse) => {
    void navigate(`/play/${modeType}/${arena.id}`)
  }

  return (
    <div className="flex flex-col gap-6 p-6">
      <LoaderState
        isLoading={isLoading}
        error={error}
        isEmpty={!arenas || arenas.length === 0}
        empty={
          <div className="text-muted-foreground mt-8 text-center">
            Для этого режима пока нет доступных арен.
          </div>
        }
        loadingFallback={
          <div className="grid gap-6 sm:grid-cols-2 lg:grid-cols-3">
            {[...Array(3)].map((_, i) => (
              <div key={i} className="overflow-hidden rounded-2xl border shadow-sm">
                <Skeleton className="aspect-video w-full" />
                <div className="space-y-2 p-4">
                  <Skeleton className="h-5 w-2/3" />
                  <Skeleton className="h-4 w-full" />
                  <Skeleton className="h-4 w-5/6" />
                </div>
              </div>
            ))}
          </div>
        }
      >
        {!!arenas?.length && (
          <div className="grid gap-6 sm:grid-cols-2 lg:grid-cols-3">
            {arenas.map((arena) => (
              <GameArenaCard key={arena.id} arena={arena} onSelect={handleArenaClick} />
            ))}
          </div>
        )}
      </LoaderState>
    </div>
  )
}
