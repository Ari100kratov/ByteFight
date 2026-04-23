import { LoaderState } from "@/components/common/LoaderState"
import { Skeleton } from "@/components/ui/skeleton"
import { GameModeCard } from "./components/GameModeCard"
import { useGameModes } from "./useGameModes"
import { useNavigate } from "react-router-dom"

export default function GameModesPage() {
  const { data: modes, isLoading, error } = useGameModes()
  const navigate = useNavigate()

  return (
    <div className="flex flex-col gap-6 p-6">
      <LoaderState
        isLoading={isLoading}
        error={error}
        empty={<div className="text-center text-muted-foreground">Режимы игры пока недоступны.</div>}
        loadingFallback={
          <div className="grid gap-8 md:grid-cols-2 xl:grid-cols-3">
            {[...Array(3)].map((_, i) => (
              <div key={i} className="overflow-hidden rounded-2xl border shadow-sm">
                <Skeleton className="aspect-video w-full" />
                <div className="space-y-3 p-5">
                  <Skeleton className="h-6 w-2/3" />
                  <Skeleton className="h-4 w-full" />
                  <Skeleton className="h-4 w-5/6" />
                </div>
              </div>
            ))}
          </div>
        }
      >
        {modes && (
          <div className="grid gap-8 md:grid-cols-2 xl:grid-cols-3">
            {modes.map((mode) => (
              <GameModeCard
                key={mode.id}
                mode={mode}
                onSelect={(mode) => navigate(`/play/${mode.slug}`)}
              />
            ))}
          </div>
        )}
      </LoaderState>
    </div>
  )
}