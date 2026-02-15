import { useParams } from "react-router-dom"
import { useDefaultLayout } from "react-resizable-panels"
import {
  ResizablePanelGroup,
  ResizablePanel,
  ResizableHandle,
} from "@/components/ui/resizable"
import CharacterCodeBlock from "../character-code-block/CharacterCodeBlock"
import { SelectCharacterCard } from "./components/SelectCharacterCard"
import { ArenaCard } from "./components/ArenaCard"
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card"
import { LoaderState } from "@/components/common/LoaderState"
import { Skeleton } from "@/components/ui/skeleton"
import { useArena } from "./hooks/useArena"
import { useCharacterStore } from "../game/state/data/character.data.store"
import { useGameSession } from "./hooks/useGameSession"
import { useArenaBreadcrumbs } from "@/shared/hooks/useArenaBreadcrumbs"

export default function GameArenaPage() {
  const { modeType, arenaId, sessionId } = useParams()

  const { data: arena, isLoading, error } = useArena(arenaId)
  const character = useCharacterStore(s => s.character)

  const rootLayout = useDefaultLayout({
    id: "game-arena-layout",
    panelIds: ["left-column", "arena-panel"],
  })

  const leftColumnLayout = useDefaultLayout({
    id: "left-column-layout",
    panelIds: ["character-panel", "code-panel"],
  })

  useArenaBreadcrumbs({ modeType, arena })
  useGameSession(sessionId)

  return (
    <div className="flex flex-col gap-6 p-4 w-full h-full">
      <LoaderState
        isLoading={isLoading}
        error={error}
        skeletonClassName="w-full h-full rounded-2xl"
        loadingFallback={
          <ResizablePanelGroup
            id="game-arena-loading-layout"
            direction="horizontal"
            className="h-full rounded-2xl border"
          >
            {/* Левая часть */}
            <ResizablePanel id="loading-left" defaultSize={40} minSize={25}>
              <ResizablePanelGroup id="game-arena-loading-left-layout" direction="vertical" className="h-full">
                <ResizablePanel id="loading-character" defaultSize={30} minSize={20}>
                  <Skeleton className="h-full w-full rounded-md" />
                </ResizablePanel>

                <ResizableHandle withHandle />

                <ResizablePanel id="loading-code" defaultSize={70} minSize={30}>
                  <Skeleton className="h-full w-full rounded-md" />
                </ResizablePanel>
              </ResizablePanelGroup>
            </ResizablePanel>

            <ResizableHandle withHandle />

            {/* Правая часть */}
            <ResizablePanel id="loading-arena" defaultSize={60} minSize={35}>
              <Skeleton className="h-full w-full rounded-md" />
            </ResizablePanel>
          </ResizablePanelGroup>
        }
      >
        <ResizablePanelGroup
          id="game-arena-layout"
          direction="horizontal"
          defaultLayout={rootLayout.defaultLayout}
          onLayoutChanged={rootLayout.onLayoutChanged}
          resizeTargetMinimumSize={{ coarse: 36, fine: 24 }}
          className="h-full rounded-2xl border"
        >
          {/* Левая часть */}
          <ResizablePanel id="left-column" defaultSize={40} minSize={25}>
            <ResizablePanelGroup
              id="left-column-layout"
              direction="vertical"
              defaultLayout={leftColumnLayout.defaultLayout}
              onLayoutChanged={leftColumnLayout.onLayoutChanged}
              resizeTargetMinimumSize={{ coarse: 36, fine: 24 }}
              className="h-full"
            >
              {/* Блок персонажа */}
              <ResizablePanel id="character-panel" defaultSize={40} minSize={25}>
                <SelectCharacterCard />
              </ResizablePanel>

              <ResizableHandle withHandle />

              {/* Блок кода */}
              <ResizablePanel id="code-panel" defaultSize={60} minSize={30}>
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
          <ResizablePanel id="arena-panel" defaultSize={60} minSize={35}>
            <ArenaCard />
          </ResizablePanel>
        </ResizablePanelGroup>
      </LoaderState>
    </div>
  )
}
