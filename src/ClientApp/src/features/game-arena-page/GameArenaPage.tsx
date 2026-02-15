import { useParams } from "react-router-dom"
import { Group, Panel, Separator, useDefaultLayout } from "react-resizable-panels"
import { GripVerticalIcon } from "lucide-react"
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
import { cn } from "@/shared/lib/utils"

function ResizeHandle() {
  return (
    <Separator
      className={cn(
        "bg-border focus-visible:ring-ring relative flex w-px items-center justify-center after:absolute after:inset-y-0 after:left-1/2 after:w-1 after:-translate-x-1/2 focus-visible:ring-1 focus-visible:ring-offset-1 focus-visible:outline-hidden aria-[orientation=vertical]:h-px aria-[orientation=vertical]:w-full aria-[orientation=vertical]:after:left-0 aria-[orientation=vertical]:after:h-1 aria-[orientation=vertical]:after:w-full aria-[orientation=vertical]:after:translate-x-0 aria-[orientation=vertical]:after:-translate-y-1/2 [&[aria-orientation=vertical]>div]:rotate-90"
      )}
    >
      <div className="bg-border z-10 flex h-4 w-3 items-center justify-center rounded-xs border">
        <GripVerticalIcon className="size-2.5" />
      </div>
    </Separator>
  )
}

export default function GameArenaPage() {
  const { modeType, arenaId, sessionId } = useParams()

  const { data: arena, isLoading, error } = useArena(arenaId)
  const character = useCharacterStore(s => s.character)

  const rootLayout = useDefaultLayout({
    id: "game-arena-layout-v2",
    panelIds: ["left-column", "arena-panel"],
  })

  const leftColumnLayout = useDefaultLayout({
    id: "left-column-layout-v2",
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
          <Group
            id="game-arena-loading-layout"
            orientation="horizontal"
            className="h-full w-full rounded-2xl border"
          >
            <Panel id="loading-left" defaultSize="40%" minSize="25%">
              <Group id="game-arena-loading-left-layout" orientation="vertical" className="h-full w-full">
                <Panel id="loading-character" defaultSize="30%" minSize="20%">
                  <Skeleton className="h-full w-full rounded-md" />
                </Panel>

                <ResizeHandle />

                <Panel id="loading-code" defaultSize="70%" minSize="30%">
                  <Skeleton className="h-full w-full rounded-md" />
                </Panel>
              </Group>
            </Panel>

            <ResizeHandle />

            <Panel id="loading-arena" defaultSize="60%" minSize="35%">
              <Skeleton className="h-full w-full rounded-md" />
            </Panel>
          </Group>
        }
      >
        <Group
          id="game-arena-layout-v2"
          orientation="horizontal"
          defaultLayout={rootLayout.defaultLayout}
          onLayoutChanged={rootLayout.onLayoutChanged}
          resizeTargetMinimumSize={{ coarse: 36, fine: 24 }}
          className="h-full w-full rounded-2xl border"
        >
          <Panel id="left-column" defaultSize="40%" minSize="25%">
            <Group
              id="left-column-layout-v2"
              orientation="vertical"
              defaultLayout={leftColumnLayout.defaultLayout}
              onLayoutChanged={leftColumnLayout.onLayoutChanged}
              resizeTargetMinimumSize={{ coarse: 36, fine: 24 }}
              className="h-full w-full"
            >
              <Panel id="character-panel" defaultSize="40%" minSize="25%">
                <SelectCharacterCard />
              </Panel>

              <ResizeHandle />

              <Panel id="code-panel" defaultSize="60%" minSize="30%">
                {character ? (
                  <div className="h-full overflow-auto">
                    <CharacterCodeBlock
                      characterId={character.id}
                      className="rounded-none md:rounded-tl-2xl border-0 border-b md:border-b-0 md:border-r"
                    />
                  </div>
                ) : (
                  <Card className="flex flex-col w-full h-full rounded-none md:rounded-tl-2xl border-0 border-b md:border-b-0 md:border-r overflow-auto">
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
              </Panel>
            </Group>
          </Panel>

          <ResizeHandle />

          <Panel id="arena-panel" defaultSize="60%" minSize="35%">
            <ArenaCard />
          </Panel>
        </Group>
      </LoaderState>
    </div>
  )
}
