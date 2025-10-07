import { useEffect, useMemo } from "react"
import { useParams } from "react-router-dom"
import { useBreadcrumbNames } from "@/layouts/BreadcrumbProvider"
import { useArenasByMode } from "./useArenasByMode"
import { LoaderState } from "@/components/common/LoaderState"
import { Skeleton } from "@/components/ui/skeleton"
import { Card, CardContent, CardTitle, CardDescription } from "@/components/ui/card"

export default function GameArenasPage() {
  const { id: modeType } = useParams<{ id: string }>()
  const { setName } = useBreadcrumbNames()

  const modeName = useMemo(() => {
    switch (modeType) {
      case "training":
        return "Тренировка"
      case "pve":
        return "PvE"
      case "pvp":
        return "PvP"
      default:
        return "Неизвестный режим"
    }
  }, [modeType])

  useEffect(() => {
    if (modeType) {
      setName(`/play/${modeType}`, modeName)
    }
  }, [modeType, modeName, setName])

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
              <Card
                key={arena.id}
                className="cursor-pointer overflow-hidden hover:shadow-lg transition-all hover:-translate-y-1"
              >
                {/* Заглушка под изображение */}
                <div className="h-40 bg-muted flex items-center justify-center text-muted-foreground text-sm">
                  Изображение арены
                </div>

                <CardContent className="p-4 space-y-2">
                  <CardTitle className="text-lg font-semibold">{arena.name}</CardTitle>
                  <CardDescription className="text-sm text-muted-foreground line-clamp-3">
                    {arena.description || "Описание отсутствует."}
                  </CardDescription>
                </CardContent>
              </Card>
            ))}
          </div>
        )}
      </LoaderState>
    </div>
  )
}
