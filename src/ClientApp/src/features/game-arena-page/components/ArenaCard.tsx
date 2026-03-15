import { Card, CardHeader, CardTitle, CardContent, CardFooter } from "@/components/ui/card"
import { Button } from "@/components/ui/button"
import { Info, RotateCcw, SwordsIcon } from "lucide-react"
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
import { useGameRuntimeStore } from "@/features/game/state/game.runtime.store"
import { isGameSessionActive } from "@/features/game/types/GameSession"
import { resetGameWorldState } from "@/features/game/state/stateReset"

export function ArenaCard() {
  const navigate = useNavigate()
  const { modeType, arenaId, sessionId } = useParams<{ modeType: string; arenaId: string; sessionId?: string }>()

  const setViewportSize = useViewportStore(s => s.setSize)
  const { showGrid, setShowGrid } = useGridStore()

  const arenaRef = useResizeObserver(size => {
    setViewportSize(size)
  }, 200)

  const arena = useArenaStore(s => s.arena)
  const character = useCharacterStore(s => s.character)
  const session = useGameRuntimeStore(s => s.session)
  const getActiveCode = useCodeEditorStore(s => s.getActiveCode)
  const { start: startLoading, isLoading } = useGameBootstrapStore()

  const { mutateAsync: startGame } = useStartGame()

  async function startSession(characterId: string) {
    if (!arenaId) {
      toast.error("Арена не выбрана")
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

    resetGameWorldState()
    startLoading()

    const newSessionId = await startGame({
      arenaId,
      mode: modeType,
      characterId,
      code: localCode.sourceCode
    })

    navigate(`/play/${modeType}/${arenaId}/${newSessionId}`)
  }

  async function handleStart() {
    if (!character) {
      toast.error("Персонаж не выбран")
      return
    }

    await startSession(character.id)
  }

  async function handleRestart() {
    const restartCharacterId = session?.characterId ?? character?.id

    if (!restartCharacterId) {
      toast.error("Персонаж не выбран")
      return
    }

    await startSession(restartCharacterId)
  }

  if (!arena) {
    return <Skeleton className="w-full h-full rounded-none md:rounded-r-2xl" />
  }

  const isSessionActive = isGameSessionActive(session)
  const canRestart = Boolean(sessionId && session && !isSessionActive)

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
          />
          <span className="text-sm text-muted-foreground">
            Показать сетку
          </span>
        </div>

        <div className="flex items-center gap-2">
          {canRestart && (
            <Button size="lg" variant="outline" disabled={isLoading} onClick={handleRestart}>
              <RotateCcw /> Повторить бой
            </Button>
          )}

          <Button size="lg" disabled={isLoading || isSessionActive} onClick={handleStart}>
            {isLoading ? "Готовимся к бою..." : <><SwordsIcon /> В бой</>}
          </Button>
        </div>
      </CardFooter>
    </Card>
  )
}
