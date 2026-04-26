import { Card, CardContent, CardDescription, CardTitle } from "@/components/ui/card"
import { Grid3X3, Skull } from "lucide-react"
import { getAssetUrl } from "@/shared/api/loadActionAssets"
import type { ArenaResponse } from "../useArenasByMode"

type Props = {
  arena: ArenaResponse
  onSelect: (arena: ArenaResponse) => void
}

export function GameArenaCard({ arena, onSelect }: Props) {
  const imageSrc = getAssetUrl(arena.imageUrl)

  return (
    <Card
      className="group cursor-pointer overflow-hidden border transition-all duration-300 hover:-translate-y-1 hover:shadow-lg"
      onClick={() => onSelect(arena)}
    >
      <div className="relative aspect-video overflow-hidden bg-muted">
        {imageSrc ? (
          <>
            <img
              src={imageSrc}
              alt={arena.name}
              loading="lazy"
              draggable={false}
              className="h-full w-full object-cover object-center will-change-transform transition-transform duration-700 ease-out group-hover:scale-[1.04]"
            />
            <div className="absolute inset-0 bg-gradient-to-t from-black/20 via-black/5 to-transparent" />
          </>
        ) : (
          <div className="flex h-full items-center justify-center text-xs text-muted-foreground">
            Изображение недоступно
          </div>
        )}
      </div>

      <CardContent className="space-y-2 p-4">
        <CardTitle className="text-lg font-semibold">
          {arena.name}
        </CardTitle>

        <CardDescription className="line-clamp-3 text-sm leading-relaxed text-muted-foreground">
          {arena.description || "Описание отсутствует."}
        </CardDescription>

        <div className="flex flex-wrap gap-2 pt-3">
          <span className="inline-flex items-center gap-2 rounded-md bg-secondary px-4 py-2 text-sm font-semibold text-secondary-foreground">
            <Grid3X3 className="shrink-0" />
            {arena.gridWidth}×{arena.gridHeight}
          </span>

          {arena.enemies.map((enemy) => (
            <span
              key={enemy.enemyId}
              className="inline-flex items-center gap-2 rounded-md border px-4 py-2 text-sm font-semibold text-foreground"
            >
              <Skull className="shrink-0" />
              {enemy.name} ×{enemy.count}
            </span>
          ))}
        </div>
      </CardContent>
    </Card>
  )
}