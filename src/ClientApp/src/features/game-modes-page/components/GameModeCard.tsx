import { Card, CardContent, CardTitle, CardDescription } from "@/components/ui/card"
import type { GameModeResponse } from "../useGameModes"

type Props = {
  mode: GameModeResponse
  onSelect: (mode: GameModeResponse) => void
}

export function GameModeCard({ mode, onSelect }: Props) {
  return (
    <Card
      className="cursor-pointer overflow-hidden hover:shadow-lg transition-all hover:-translate-y-1"
      onClick={() => onSelect(mode)}
    >
      {/* Будущее место для изображения (пока placeholder) */}
      <div className="h-40 bg-muted flex items-center justify-center text-muted-foreground text-sm">
        {/* Позже сюда можно вставить <img src={mode.imageUrl} /> */}
        Изображение
      </div>

      <CardContent className="p-4 space-y-2">
        <CardTitle className="text-lg font-semibold">{mode.name}</CardTitle>
        <CardDescription className="text-sm text-muted-foreground line-clamp-3">
          {mode.description}
        </CardDescription>
      </CardContent>
    </Card>
  )
}
