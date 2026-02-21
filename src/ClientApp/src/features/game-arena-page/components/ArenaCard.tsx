import { Card, CardHeader, CardTitle, CardContent, CardFooter, CardDescription } from "@/components/ui/card"
import { Button } from "@/components/ui/button"
import { SwordsIcon } from "lucide-react"
import { Game } from "@/features/game/Game"
import { useArenaStore } from "@/features/game/state/data/arena.data.store"
import { Skeleton } from "@/components/ui/skeleton"
import { useCharacterStore } from "@/features/game/state/data/character.data.store"
import { useStartGame } from "@/features/game/api/useStartGame"
import { toast } from "sonner"
import { useNavigate, useParams } from "react-router-dom"
import { useGameBootstrapStore } from "@/features/game/state/game.bootstrap.store"
import { useCodeEditorStore } from "@/features/character-code-block/state/codeEditor.store"
import { useResizeObserver } from "@/shared/hooks/useResizeObserver"
import { useViewportStore } from "@/features/game/state/viewport/viewport.store"
import { useGridStore } from "@/features/game/state/game/grid.state.store"
import { Switch } from "@/components/ui/switch"

export function ArenaCard() {
  const navigate = useNavigate()
  const { modeType, arenaId } = useParams<{ modeType: string; arenaId: string }>()

  const setViewportSize = useViewportStore(s => s.setSize)
  const { showGrid, setShowGrid } = useGridStore()

  const arenaRef = useResizeObserver(size => {
    setViewportSize(size)
  }, 200)

  const arena = useArenaStore(s => s.arena)
  const character = useCharacterStore(s => s.character)
  const getActiveCode = useCodeEditorStore(s => s.getActiveCode)
  const { start: startLoading, isLoading } = useGameBootstrapStore()

  const { mutateAsync: startGame } = useStartGame()

  async function handleStart() {
    if (!arenaId) {
      toast.error("Арена не выбрана")
      return
    }

    if (!character) {
      toast.error("Персонаж не выбран")
      return
    }

    if (!modeType) {
      toast.error("Режим игры не выбран")
      return
    }

    const localCode = getActiveCode()
    if (!localCode) {
      toast.error("Пользовательский код не задан")
      return
    }

    startLoading()

    const sessionId = await startGame({
      arenaId: arenaId,
      mode: modeType,
      characterId: character.id,
      code: localCode.sourceCode
    })

    navigate(`/play/${modeType}/${arenaId}/${sessionId}`)
  }


  if (!arena) {
    return <Skeleton className="w-full h-full rounded-none md:rounded-r-2xl" />
  }

  return (
    <Card className="h-full rounded-none md:rounded-r-2xl flex flex-col overflow-hidden">
      <CardHeader>
        <CardTitle>{arena.name}</CardTitle>
        <CardDescription>{arena.description}</CardDescription>
      </CardHeader>
      <CardContent className="flex-1 p-0 flex justify-center items-center">
        <div ref={arenaRef} className="w-full h-full">
          <Game />
        </div>
      </CardContent>
      <CardFooter className="flex justify-between items-center">
        <div className="flex items-center gap-2">
          <Switch
            checked={showGrid}
            onCheckedChange={setShowGrid}
          />
          <span className="text-sm text-muted-foreground">
            Показать сетку
          </span>
        </div>
        <Button size="lg" disabled={isLoading} onClick={handleStart}>
          {isLoading ? "Готовимся к бою..." : <><SwordsIcon /> В бой</>}
        </Button>
      </CardFooter>
    </Card>
  )
}
