import { Link } from "react-router-dom"
import { Button } from "@/components/ui/button"

export default function NotFoundPage() {
  return (
    <div className="flex h-full min-h-0 items-center justify-center overflow-auto px-4 py-6">
      <div className="w-full max-w-2xl space-y-6 text-center">
        <div className="space-y-2">
          <h1 className="text-6xl font-black tracking-tight sm:text-7xl">404</h1>

          <p className="text-2xl font-bold sm:text-3xl">Ты упёрся в границу карты</p>

          <p className="text-muted-foreground text-sm sm:text-base">
            Этот маршрут не существует, ещё не реализован или просто решил исчезнуть в самый
            неподходящий момент.
          </p>
        </div>

        <div className="bg-muted/30 rounded-xl border p-5 font-mono text-sm sm:p-6 sm:text-base">
          <div className="space-y-2 text-left">
            <p>{"> move right"}</p>
            <p>{"> move right"}</p>
            <p className="text-destructive font-semibold">{"> ERROR: collision detected"}</p>
            <p>{"> checking route..."}</p>
            <p className="text-muted-foreground">{"> route not found"}</p>
          </div>

          <div className="mt-5 flex justify-center gap-1">
            {Array.from({ length: 12 }).map((_, i) => (
              <div key={i} className="bg-foreground/80 h-8 w-3 rounded-sm sm:h-10 sm:w-4" />
            ))}
          </div>

          <div className="bg-background/70 text-muted-foreground mt-5 rounded-lg border px-4 py-3 text-sm">
            <span className="text-foreground font-semibold">NPC:</span> Туда нельзя. Разработчик ещё
            не дописал.
          </div>
        </div>

        <div className="flex flex-col justify-center gap-3 sm:flex-row">
          <Button asChild size="lg">
            <Link to="/play">Вернуться в игру ⚔️</Link>
          </Button>

          <Button
            variant="ghost"
            size="lg"
            onClick={() => {
              window.history.back()
            }}
          >
            Назад
          </Button>
        </div>
      </div>
    </div>
  )
}
