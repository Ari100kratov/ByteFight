import { Link } from "react-router-dom"
import { Button } from "@/components/ui/button"

export default function NotFoundPage() {
  return (
    <div className="flex h-full min-h-0 items-center justify-center overflow-auto px-4 py-6">
      <div className="w-full max-w-2xl text-center space-y-6">
        <div className="space-y-2">
          <h1 className="text-6xl font-black tracking-tight sm:text-7xl">
            404
          </h1>

          <p className="text-2xl font-bold sm:text-3xl">
            Ты упёрся в границу карты
          </p>

          <p className="text-sm text-muted-foreground sm:text-base">
            Этот маршрут не существует, ещё не реализован или просто решил
            исчезнуть в самый неподходящий момент.
          </p>
        </div>

        <div className="rounded-xl border bg-muted/30 p-5 font-mono text-sm sm:p-6 sm:text-base">
          <div className="space-y-2 text-left">
            <p>{"> move right"}</p>
            <p>{"> move right"}</p>
            <p className="font-semibold text-destructive">
              {"> ERROR: collision detected"}
            </p>
            <p>{"> checking route..."}</p>
            <p className="text-muted-foreground">{"> route not found"}</p>
          </div>

          <div className="mt-5 flex justify-center gap-1">
            {Array.from({ length: 12 }).map((_, i) => (
              <div
                key={i}
                className="h-8 w-3 rounded-sm bg-foreground/80 sm:h-10 sm:w-4"
              />
            ))}
          </div>

          <div className="mt-5 rounded-lg border bg-background/70 px-4 py-3 text-sm text-muted-foreground">
            <span className="font-semibold text-foreground">NPC:</span> Туда
            нельзя. Разработчик ещё не дописал.
          </div>
        </div>

        <div className="flex flex-col justify-center gap-3 sm:flex-row">
          <Button asChild size="lg">
            <Link to="/play">Вернуться в игру ⚔️</Link>
          </Button>

          <Button
            variant="ghost"
            size="lg"
            onClick={() => window.history.back()}
          >
            Назад
          </Button>
        </div>
      </div>
    </div>
  )
}