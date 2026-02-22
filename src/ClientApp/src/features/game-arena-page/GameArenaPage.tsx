import { useParams } from "react-router-dom"
import { useDefaultLayout } from "react-resizable-panels"

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
import { Group, Panel, Separator } from "@/components/ui/resizable"

export default function GameArenaPage() {
  const { modeType, arenaId, sessionId } = useParams()

  const { data: arena, isLoading, error } = useArena(arenaId)
  const character = useCharacterStore(s => s.character)

  useArenaBreadcrumbs({ modeType, arena })
  useGameSession(sessionId)

  const {
    defaultLayout,
    onLayoutChanged
  } = useDefaultLayout({
    id: "game-arena-layout"
  })

  return (
    <div className="flex flex-col gap-4 w-full h-full">
      <LoaderState
        isLoading={isLoading}
        error={error}
        skeletonClassName="w-full h-full rounded-2xl"
        loadingFallback={
          <Group orientation="horizontal">
            <Panel defaultSize="40%">
              <Group orientation="vertical">
                <Panel defaultSize="30%" className="p-2">
                  <Skeleton className="h-full w-full rounded-md" />
                </Panel>

                <Separator withHandle />

                <Panel defaultSize="70%" className="p-2">
                  <Skeleton className="h-full w-full rounded-md" />
                </Panel>
              </Group>
            </Panel>

            <Separator withHandle />

            <Panel defaultSize="60%" minSize="30%" className="p-2">
              <Skeleton className="h-full w-full rounded-md" />
            </Panel>
          </Group>
        }
      >
        <Group
          orientation="horizontal"
          defaultLayout={defaultLayout}
          onLayoutChanged={onLayoutChanged}
        >
          {/* Левая часть */}
          <Panel
            id="left-panel"
            defaultSize="40%"
          >
            <Group orientation="vertical">
              {/* Блок персонажа */}
              <Panel
                id="character-panel"
                defaultSize="40%"
                className="p-2"
              >
                <SelectCharacterCard />
              </Panel>

              <Separator withHandle />

              {/* Блок кода */}
              <Panel
                id="code-panel"
                defaultSize="60%"
                className="p-2"
              >
                {character ? (
                  <div className="h-full overflow-auto">
                    <CharacterCodeBlock
                      characterId={character.id}
                    />
                  </div>
                ) : (
                  <Card className="flex flex-col w-full h-full overflow-auto">
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

          <Separator withHandle />

          {/* Правая часть — арена */}
          <Panel
            id="arena-panel"
            defaultSize="60%"
            minSize="30%"
            className="p-2"
          >
            <ArenaCard />
          </Panel>
        </Group>
      </LoaderState>
    </div>
  )
}