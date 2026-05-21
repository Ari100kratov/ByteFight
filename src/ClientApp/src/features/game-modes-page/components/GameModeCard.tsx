import { Card, CardContent, CardTitle, CardDescription } from "@/components/ui/card"
import type { GameModeResponse } from "../useGameModes"
import { getAssetUrl } from "@/shared/api/loadActionAssets"

interface Props {
  mode: GameModeResponse
  onSelect: (mode: GameModeResponse) => void
}

export function GameModeCard({ mode, onSelect }: Props) {
  const imageSrc = getAssetUrl(mode.imageUrl)

  return (
    <Card
      className="group cursor-pointer overflow-hidden border transition-all duration-300 hover:-translate-y-1 hover:shadow-xl"
      onClick={() => {
        onSelect(mode)
      }}
    >
      <div className="bg-muted relative aspect-video overflow-hidden">
        {imageSrc ? (
          <>
            <img
              src={imageSrc}
              alt={mode.name}
              loading="lazy"
              draggable={false}
              className="h-full w-full object-cover object-center transition-transform duration-700 ease-out will-change-transform group-hover:scale-[1.05]"
            />
            <div className="absolute inset-0 bg-gradient-to-t from-black/20 via-black/5 to-transparent" />
          </>
        ) : (
          <div className="text-muted-foreground flex h-full items-center justify-center text-sm">
            Изображение недоступно
          </div>
        )}
      </div>

      <CardContent className="space-y-3 p-5">
        <CardTitle className="text-xl font-semibold">{mode.name}</CardTitle>

        <CardDescription className="text-muted-foreground line-clamp-3 text-sm">
          {mode.description}
        </CardDescription>
      </CardContent>
    </Card>
  )
}
