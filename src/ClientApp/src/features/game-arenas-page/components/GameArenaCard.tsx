import { Card, CardContent, CardTitle, CardDescription } from "@/components/ui/card"
import type { ArenaResponse } from "../useArenasByMode"

interface GameArenaCardProps {
  arena: ArenaResponse
  onClick: (arena: ArenaResponse) => void
}

export function GameArenaCard({ arena, onClick }: GameArenaCardProps) {
  return (
    <Card
      onClick={() => onClick(arena)}
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
  )
}
