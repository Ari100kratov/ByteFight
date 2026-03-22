import { useParams } from "react-router-dom"
import { useDefaultLayout } from "react-resizable-panels"

import CharacterCodeBlock from "../character-code-block/CharacterCodeBlock"
import { SelectCharacterCard } from "./components/select-character-card/SelectCharacterCard"
import { ArenaCard } from "./components/ArenaCard"
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card"
import { LoaderState } from "@/components/common/LoaderState"
import { Skeleton } from "@/components/ui/skeleton"
import { useArena } from "./hooks/useArena"
import { useCharacterStore } from "../game/state/data/character.data.store"
import { useGameSession } from "./hooks/useGameSession"
import { useArenaBreadcrumbs } from "@/shared/hooks/useArenaBreadcrumbs"
import { Group, Panel, Separator } from "@/components/ui/resizable"
import { CombatLogPanel } from "../game/combat-log/CombatLogPanel"
import { useEffect, useRef } from "react"

function GameArenaPageSkeleton() {
  return (
    <Group orientation="horizontal">
      {/* Левая часть */}
      <Panel id="left-panel-skeleton" defaultSize="30%">
        <Group orientation="vertical">
          <Panel
            id="character-panel-skeleton"
            defaultSize="40%"
            className="p-2"
          >
            <Skeleton className="h-full w-full rounded-md" />
          </Panel>

          <Separator withHandle />

          <Panel
            id="code-panel-skeleton"
            defaultSize="60%"
            className="p-2"
          >
            <Skeleton className="h-full w-full rounded-md" />
          </Panel>
        </Group>
      </Panel>

      <Separator withHandle />

      {/* Правая часть */}
      <Panel
        id="right-panel-skeleton"
        defaultSize="70%"
        minSize="30%"
      >
        <Group orientation="horizontal">
          <Panel
            id="arena-panel-skeleton"
            defaultSize="70%"
            minSize="40%"
            className="p-2"
          >
            <Skeleton className="h-full w-full rounded-md" />
          </Panel>

          <Separator withHandle />

          <Panel
            id="combat-log-panel-skeleton"
            defaultSize="30%"
            className="p-2"
          >
            <Skeleton className="h-full w-full rounded-md" />
          </Panel>
        </Group>
      </Panel>
    </Group>
  )
}

export default function GameArenaPage() {
  const { modeType, arenaId, sessionId } = useParams()

  // Да, это костыль — но зато рабочий.
  // Я честно пытался нормально сбрасывать состояние при выходе из сессии,
  // но из-за анимаций, асинхронных событий с сервера, глобальных сторов
  // и всевозможных гонок состояний всё это превращается в ад:
  // где-то не доинициализировалось, где-то остался старый controller,
  // где-то не применились текстуры, где-то отвалились подписки и т.д.
  //
  // В какой-то момент стало понятно, что пытаться это всё аккуратно
  // синхронизировать — это бесконечная борьба с edge-case'ами.
  //
  // Поэтому делаем максимально тупо, но надежно:
  // при исчезновении sessionId просто делаем полный reload страницы.
  // Это гарантированно очищает ВСЁ состояние (включая сторы, registry,
  // Pixi-инстансы и прочие сайд-эффекты) и возвращает приложение
  // в полностью консистентное состояние.
  //
  // Если когда-нибудь захочется "сделать правильно" — удачи тому герою :)
  const prevSessionIdRef = useRef<string | undefined>(sessionId)
  useEffect(() => {
    const prevSessionId = prevSessionIdRef.current

    if (prevSessionId && !sessionId) {
      window.location.reload()
      return
    }

    prevSessionIdRef.current = sessionId
  }, [sessionId])

  const { data: arena, isLoading, error } = useArena(arenaId)
  const character = useCharacterStore(s => s.character)

  useArenaBreadcrumbs({ modeType, arena })
  useGameSession(sessionId)

  const {
    defaultLayout: rootDefaultLayout,
    onLayoutChanged: onRootLayoutChanged
  } = useDefaultLayout({
    id: "game-arena-layout"
  })

  const {
    defaultLayout: leftDefaultLayout,
    onLayoutChanged: onLeftLayoutChanged
  } = useDefaultLayout({
    id: "game-arena-left-layout"
  })

  const {
    defaultLayout: rightDefaultLayout,
    onLayoutChanged: onRightLayoutChanged
  } = useDefaultLayout({
    id: "game-arena-right-layout"
  })

  return (
    <div className="flex flex-col gap-4 w-full h-full">
      <LoaderState
        isLoading={isLoading}
        error={error}
        skeletonClassName="w-full h-full rounded-2xl"
        loadingFallback={<GameArenaPageSkeleton />}
      >
        <Group
          orientation="horizontal"
          defaultLayout={rootDefaultLayout}
          onLayoutChanged={onRootLayoutChanged}
        >
          {/* Левая часть */}
          <Panel id="left-panel" defaultSize="30%">
            <Group
              orientation="vertical"
              defaultLayout={leftDefaultLayout}
              onLayoutChanged={onLeftLayoutChanged}
            >
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
                    <CharacterCodeBlock characterId={character.id} />
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

          {/* Правая часть */}
          <Panel
            id="right-panel"
            defaultSize="70%"
            minSize="30%"
          >
            <Group
              orientation="horizontal"
              defaultLayout={rightDefaultLayout}
              onLayoutChanged={onRightLayoutChanged}
            >
              {/* Арена */}
              <Panel
                id="arena-panel"
                defaultSize="70%"
                minSize="40%"
                className="p-2"
              >
                <ArenaCard />
              </Panel>

              <Separator withHandle />

              {/* Журнал боя */}
              <Panel
                id="combat-log-panel"
                defaultSize="30%"
                className="p-2"
              >
                <CombatLogPanel />
              </Panel>
            </Group>
          </Panel>
        </Group>
      </LoaderState>
    </div>
  )
}