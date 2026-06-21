import { useState } from "react"
import { Card, CardHeader, CardTitle, CardContent, CardFooter } from "@/components/ui/card"
import { Button } from "@/components/ui/button"
import { Info, LogOut, Minus, Plus, SwordsIcon } from "lucide-react"
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
import {
  ANIMATION_SPEED_PERCENT_MAX,
  ANIMATION_SPEED_PERCENT_MIN,
  ANIMATION_SPEED_PERCENT_STEP,
  useArenaPlaybackStore,
} from "@/features/game/state/game/playback.state.store"
import { Switch } from "@/components/ui/switch"
import { Tooltip, TooltipContent, TooltipTrigger } from "@/components/ui/tooltip"
import { useArenaBattleState } from "./hooks/useArenaBattleState"
import { BattleResultOverlay } from "../battle-result-overlay/BattleResultOverlay"
import { useArenaBattleResult } from "./hooks/useArenaBattleResult"
import { BattleResultTrigger } from "../battle-result-overlay/BattleResultTrigger"
import { formatModeNameByString } from "@/shared/types/modeNames"
import { GameStartErrorDialog } from "./components/GameStartErrorDialog"
import { mapStartGameError, type StartGameUiError } from "@/features/game/api/startGame.errors"
import { useUnsavedChangesConfirm } from "./hooks/useUnsavedChangesDialog"

export function ArenaCard() {
  const navigate = useNavigate()
  const { modeType, arenaId } = useParams<{ modeType: string; arenaId: string }>()

  const setViewportSize = useViewportStore((s) => s.setSize)
  const { showGrid, setShowGrid } = useGridStore()
  const animationSpeedPercent = useArenaPlaybackStore((s) => s.animationSpeedPercent)
  const adjustAnimationSpeedPercent = useArenaPlaybackStore((s) => s.adjustAnimationSpeedPercent)

  const arenaRef = useResizeObserver((size) => {
    setViewportSize(size)
  }, 200)

  const arena = useArenaStore((s) => s.arena)
  const character = useCharacterStore((s) => s.character)
  const getActiveCode = useCodeEditorStore((s) => s.getActiveCode)
  const { start: startLoading, end: endLoading, isLoading } = useGameBootstrapStore()

  const { mutateAsync: startGame } = useStartGame()
  const { isBattleBusy, hasSession } = useArenaBattleState()

  const [startError, setStartError] = useState<StartGameUiError | null>(null)

  const isStartDisabled = isLoading || isBattleBusy

  const startButtonText = (() => {
    if (isLoading) return "Готовимся к бою..."
    if (isBattleBusy) return "Идет бой"
    if (hasSession) return "Выйти из боя"
    return "В бой"
  })()

  const startButtonIcon = (() => {
    if (isLoading || isBattleBusy) return <SwordsIcon />
    if (hasSession) return <LogOut />
    return <SwordsIcon />
  })()

  function leaveBattle() {
    if (!modeType || !arenaId) {
      return
    }

    void navigate(`/play/${modeType}/${arenaId}`)
  }

  const { confirm: confirmLeaveBattle, dialog: leaveBattleDialog } = useUnsavedChangesConfirm({
    onConfirm: leaveBattle,
  })

  async function handleStart() {
    if (isStartDisabled) {
      return
    }

    if (hasSession) {
      confirmLeaveBattle()
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
    if (!localCode?.sourceCode.trim()) {
      toast.error("Пользовательский код не задан")
      return
    }

    try {
      startLoading()

      const sessionId = await startGame({
        arenaId,
        mode: modeType,
        characterId: character.id,
        code: localCode.sourceCode,
      })

      void navigate(`/play/${modeType}/${arenaId}/${sessionId}`)
    } catch (error) {
      const mappedError = mapStartGameError(error)
      setStartError(mappedError)
      toast.error(mappedError.title)
      endLoading()
    }
  }

  const { resultView, canShowResult, isResultOpen, openResult, closeResult } =
    useArenaBattleResult()

  const handleResultTriggerClick = () => {
    if (isResultOpen) {
      closeResult()
      return
    }

    openResult()
  }

  if (!arena) {
    return <Skeleton className="h-full w-full rounded-none md:rounded-r-2xl" />
  }

  return (
    <>
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

        <CardContent className="min-h-0 flex-1 overflow-hidden p-0">
          <div className="relative h-full min-h-0 w-full overflow-hidden">
            <div ref={arenaRef} className="h-full min-h-0 w-full overflow-hidden">
              <Game />
            </div>

            {canShowResult && isResultOpen && resultView && (
              <BattleResultOverlay
                title={resultView.title}
                description={resultView.description}
                tone={resultView.tone}
                Icon={resultView.Icon}
                totalTurns={resultView.totalTurns}
                startedAt={resultView.startedAt}
                endedAt={resultView.endedAt}
                characterName={character?.name}
                characterClassName={character?.spec.className}
                characterSpecName={character?.spec.name}
                arenaName={arena.name}
                arenaModeName={modeType ? formatModeNameByString(modeType) : null}
                onClose={closeResult}
              />
            )}
          </div>
        </CardContent>

        <CardFooter className="flex shrink-0 flex-wrap items-center justify-between gap-3">
          <div className="flex flex-wrap items-center gap-x-4 gap-y-2">
            <div className="flex items-center gap-2">
              <span className="text-muted-foreground text-sm">Сетка</span>
              <Switch
                checked={showGrid}
                onCheckedChange={setShowGrid}
                aria-label="Показать сетку"
              />
            </div>

            <div className="flex items-center gap-1.5">
              <span className="text-muted-foreground text-sm">Скорость</span>
              <Button
                type="button"
                variant="outline"
                size="icon"
                className="size-8"
                aria-label="Уменьшить скорость анимаций"
                disabled={animationSpeedPercent <= ANIMATION_SPEED_PERCENT_MIN}
                onClick={() => {
                  adjustAnimationSpeedPercent(-ANIMATION_SPEED_PERCENT_STEP)
                }}
              >
                <Minus className="size-4" />
              </Button>
              <span className="text-muted-foreground w-12 text-center text-sm tabular-nums">
                {animationSpeedPercent}%
              </span>
              <Button
                type="button"
                variant="outline"
                size="icon"
                className="size-8"
                aria-label="Увеличить скорость анимаций"
                disabled={animationSpeedPercent >= ANIMATION_SPEED_PERCENT_MAX}
                onClick={() => {
                  adjustAnimationSpeedPercent(ANIMATION_SPEED_PERCENT_STEP)
                }}
              >
                <Plus className="size-4" />
              </Button>
            </div>
          </div>

          <div className="flex items-center gap-2">
            {canShowResult && resultView && (
              <BattleResultTrigger
                title={resultView.title}
                tone={resultView.tone}
                Icon={resultView.Icon}
                onClick={handleResultTriggerClick}
              />
            )}

            <Button
              size="lg"
              disabled={isStartDisabled}
              onClick={() => {
                void handleStart()
              }}
              aria-busy={isLoading}
            >
              <>
                {startButtonIcon}
                {startButtonText}
              </>
            </Button>
          </div>
        </CardFooter>
      </Card>

      <GameStartErrorDialog
        open={!!startError}
        onOpenChange={(open) => {
          if (!open) {
            setStartError(null)
          }
        }}
        title={startError?.title ?? ""}
        detail={startError?.detail ?? ""}
      />
      {leaveBattleDialog}
    </>
  )
}
