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

  const handleArenaClick = (arena: ArenaResponse) => {
    navigate(`/play/${modeType}/${arena.id}`)
  }

  const { data: arenas, isLoading, error } = useArenasByMode(modeType)

  return (
    <div className="p-6 flex flex-col gap-6">
      <LoaderState
        isLoading={isLoading}
        error={error}
        empty={
          <div className="text-muted-foreground text-center mt-8">
            Для этого режима пока нет доступных арен.
          </div>
        }
        loadingFallback={
          <div className="grid gap-6 sm:grid-cols-2 lg:grid-cols-3">
            {[...Array(3)].map((_, i) => (
              <div key={i} className="flex flex-col rounded-2xl overflow-hidden shadow-sm">
                <Skeleton className="h-40 w-full" />
                <div className="p-4 space-y-2">
                  <Skeleton className="h-5 w-2/3" />
                  <Skeleton className="h-4 w-full" />
                  <Skeleton className="h-4 w-5/6" />
                </div>
              </div>
            ))}
          </div>
        }
      >
        {arenas?.length && (
          <div className="grid gap-6 sm:grid-cols-2 lg:grid-cols-3">
            {arenas.map((arena) => (
              <GameArenaCard
                key={arena.id}
                arena={arena}
                onClick={handleArenaClick}
              />
            ))}
          </div>
        )}
      </LoaderState>
    </div>
  )
}
