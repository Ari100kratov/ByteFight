import { useEffect } from "react"
import { useParams } from "react-router-dom"
import {
  ResizablePanelGroup,
  ResizablePanel,
  ResizableHandle,
} from "@/components/ui/resizable"
import { useBreadcrumbNames } from "@/layouts/BreadcrumbProvider"
import CharacterCodeBlock from "../character-code-block/CharacterCodeBlock"
import { SelectCharacterCard } from "./components/SelectCharacterCard"
import { ArenaCard } from "./components/ArenaCard"
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card"
import { LoaderState } from "@/components/common/LoaderState"
import { Skeleton } from "@/components/ui/skeleton"
import { ModeNames } from "../game-arenas-page/types"
import { useArena } from "./hooks/useArena"
import { useCharacterStore } from "../game/state/data/character.data.store"
import { resetGameStores } from "../game/state/stateReset"

export default function GameArenaPage() {
  const { modeType, arenaId } = useParams<{ modeType: string; arenaId: string }>()
  const { setName } = useBreadcrumbNames()
  const { data: arena, isLoading, error } = useArena(arenaId)

  const character = useCharacterStore(s => s.character)

  useEffect(() => {
    return () => {
      resetGameStores()
    }
  }, [])

  useEffect(() => {
    if (modeType) {
      const modeName = ModeNames[modeType] ?? "Неизвестный режим"
      setName(`/play/${modeType}`, modeName)
    }
  }, [modeType, setName])

  useEffect(() => {
    if (arena) {
      setName(`/play/${modeType}/${arena.id}`, arena.name)
    }
  }, [arena, modeType, setName])

  return (
    <div className="flex flex-col gap-6 p-4 w-full h-full">
      <LoaderState
        isLoading={isLoading}
        error={error}
        skeletonClassName="w-full h-full rounded-2xl"
        loadingFallback={
          <ResizablePanelGroup direction="horizontal" className="h-full rounded-2xl border">
            {/* Левая часть */}
            <ResizablePanel defaultSize={40}>
              <ResizablePanelGroup direction="vertical" className="h-full">
                <ResizablePanel defaultSize={30}>
                  <Skeleton className="h-full w-full rounded-md" />
                </ResizablePanel>

                <ResizableHandle withHandle />

                <ResizablePanel defaultSize={70}>
                  <Skeleton className="h-full w-full rounded-md" />
                </ResizablePanel>
              </ResizablePanelGroup>
            </ResizablePanel>

            <ResizableHandle withHandle />

            {/* Правая часть */}
            <ResizablePanel defaultSize={60}>
              <Skeleton className="h-full w-full rounded-md" />
            </ResizablePanel>
          </ResizablePanelGroup>
        }
      >
        <ResizablePanelGroup direction="horizontal" className="h-full rounded-2xl border">
          {/* Левая часть */}
          <ResizablePanel defaultSize={40}>
            <ResizablePanelGroup direction="vertical" className="h-full">
              {/* Блок персонажа */}
              <ResizablePanel defaultSize={40}>
                <SelectCharacterCard />
              </ResizablePanel>

              <ResizableHandle withHandle />

              {/* Блок кода */}
              <ResizablePanel defaultSize={60}>
                {character ? (
                  <div className="h-full overflow-auto">
                    <CharacterCodeBlock
                      characterId={character.id}
                      className="rounded-none md:rounded-tl-2xl border-0 border-b md:border-b-0 md:border-r"
                    />
                  </div>
                ) : (
                  <Card className="flex flex-col w-full h-full overflow-auto rounded-none md:rounded-tl-2xl border-0 border-b md:border-b-0 md:border-r overflow-auto">
                    <CardHeader>
                      <CardTitle>Поведение</CardTitle>
                    </CardHeader>
                    <CardContent>
                      <div className="text-muted-foreground text-center p-4">
                        Необходимо выбрать персонажа.
                      </div>
                    </CardContent>
                  </Card>
                )}
              </ResizablePanel>
            </ResizablePanelGroup>
          </ResizablePanel>

          <ResizableHandle withHandle />

          {/* Правая часть — арена */}
          <ResizablePanel defaultSize={60}>
            <ArenaCard />
          </ResizablePanel>
        </ResizablePanelGroup>
      </LoaderState>
    </div>
  )
}
