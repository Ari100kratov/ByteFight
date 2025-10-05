import { LoaderState } from "@/components/common/LoaderState"
import { Skeleton } from "@/components/ui/skeleton"
import { GameModeCard } from "./components/GameModeCard"
import { useGameModes } from "./useGameModes"
import { useNavigate } from "react-router-dom"

export default function GameModesPage() {
  const { data: modes, isLoading, error } = useGameModes()
  const navigate = useNavigate()

  return (
    <div className="flex flex-col gap-4 p-6">
      <LoaderState
        isLoading={isLoading}
        error={error}
        empty={<div className="text-muted-foreground text-center">Режимы игры пока недоступны.</div>}
        loadingFallback={
          <div className="grid gap-6 sm:grid-cols-2 lg:grid-cols-3">
            {[...Array(3)].map((_, i) => (
              <div key={i} className="flex flex-col rounded-2xl overflow-hidden shadow-sm">
                <Skeleton className="h-40 w-full" /> {/* Заглушка под изображение */}
                <div className="p-4 space-y-2">
                  <Skeleton className="h-5 w-2/3" /> {/* Название */}
                  <Skeleton className="h-4 w-full" /> {/* Описание */}
                  <Skeleton className="h-4 w-5/6" />
                </div>
              </div>
            ))}
          </div>
        }
      >
        {modes && (
          <div className="grid gap-6 sm:grid-cols-2 lg:grid-cols-3">
            {modes.map((mode) => (
              <GameModeCard 
              key={mode.id} 
              mode={mode} 
              onSelect={(mode) => navigate(`/play/${mode.slug}`)} />
            ))}
          </div>
        )}
      </LoaderState>
    </div>
  )
}
