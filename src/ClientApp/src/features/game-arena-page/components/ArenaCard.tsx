import { Card, CardHeader, CardTitle, CardContent, CardFooter } from "@/components/ui/card"
import { Button } from "@/components/ui/button"
import { Info, SwordsIcon } from "lucide-react"
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
import { Tooltip, TooltipContent, TooltipTrigger } from "@/components/ui/tooltip"
import { useArenaBattleState } from "../hooks/useArenaBattleState"

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

  const { isBattleBusy, hasSession } = useArenaBattleState()
  const isStartDisabled = isLoading || isBattleBusy

  const startButtonText = (() => {
    if (isLoading) return "Готовимся к бою..."
    if (isBattleBusy) return "Идет бой"
    if (hasSession) return "Снова в бой"

    return "В бой"
  })()

  async function handleStart() {
    if (isStartDisabled) {
      return
    }

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
      arenaId,
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
    <Card className="flex h-full min-h-0 flex-col overflow-hidden">
      <CardHeader className="shrink-0">
        <div className="flex items-center gap-2">
          <CardTitle>{arena.name}</CardTitle>

          {arena.description && (
            <Tooltip>
              <TooltipTrigger asChild>
                <button
                  type="button"
                  className="text-muted-foreground hover:text-foreground transition-colors"
                  aria-label="Описание арены"
                >
                  <Info className="h-4 w-4" />
                </button>
              </TooltipTrigger>
              <TooltipContent side="right" className="max-w-xs text-sm">
                {arena.description}
              </TooltipContent>
            </Tooltip>
          )}
        </div>
      </CardHeader>

      <CardContent className="flex-1 min-h-0 overflow-hidden p-0">
        <div
          ref={arenaRef}
          className="h-full w-full min-h-0 overflow-hidden"
        >
          <Game />
        </div>
      </CardContent>

      <CardFooter className="shrink-0 flex justify-between items-center gap-4">
        <div className="flex items-center gap-2">
          <Switch
            checked={showGrid}
            onCheckedChange={setShowGrid}
            aria-label="Показать сетку"
          />
          <span className="text-sm text-muted-foreground">
            Показать сетку
          </span>
        </div>

        <Button
          size="lg"
          disabled={isStartDisabled}
          onClick={handleStart}
          aria-busy={isLoading}
        >
          <>
            <SwordsIcon />
            {startButtonText}
          </>
        </Button>
      </CardFooter>
    </Card>
  )
}