import { useEffect } from "react"
import { useParams } from "react-router-dom"
import { useDefaultLayout } from "react-resizable-panels"
import { Button } from "@/components/ui/button"
import {
  Card,
  CardContent,
  CardFooter,
  CardHeader,
  CardTitle,
} from "@/components/ui/card"
import { Input } from "@/components/ui/input"
import { Label } from "@/components/ui/label"
import { Skeleton } from "@/components/ui/skeleton"
import { useBreadcrumbNames } from "@/layouts/BreadcrumbProvider"
import CharacterCodeBlock from "@/features/character-code-block/CharacterCodeBlock"
import { LoaderState } from "@/components/common/LoaderState"
import { useCharacter } from "./useCharacter"
import { CharacterClassSelector } from "../character-class-selector/CharacterClassSelector"
import { Group, Panel, Separator } from "@/components/ui/resizable"

export default function CharacterPage() {
  const { id } = useParams<{ id: string }>()
  const { data: character, isLoading, error } = useCharacter(id)
  const { setName } = useBreadcrumbNames()

  const { defaultLayout, onLayoutChanged } = useDefaultLayout({
    id: "character-layout"
  })

  useEffect(() => {
    if (character) {
      setName(`/characters/${character.id}`, character.name)
    }
  }, [character, setName])

  return (
    <div className="flex flex-col gap-4 w-full h-full">
      <LoaderState
        isLoading={isLoading}
        error={error}
        skeletonClassName="w-full h-full rounded-2xl"
        loadingFallback={
          <Group orientation="horizontal">
            {/* Левая часть */}
            <Panel defaultSize="40%">
              <Group orientation="vertical">
                <Panel defaultSize="40%" className="p-2">
                  <Skeleton className="h-full w-full rounded-md" />
                </Panel>
                <Separator withHandle />
                <Panel defaultSize="60%" className="p-2 flex-1">
                  <Skeleton className="h-full w-full rounded-md" />
                </Panel>
              </Group>
            </Panel>

            <Separator withHandle />

            {/* Правая часть */}
            <Panel defaultSize="60%" className="p-2">
              <Skeleton className="h-full w-full rounded-md" />
            </Panel>
          </Group>
        }
      >
        {character && (
          <Group
            orientation="horizontal"
            defaultLayout={defaultLayout}
            onLayoutChanged={onLayoutChanged}
          >
            {/* Левая часть */}
            <Panel
              id="left-panel"
              defaultSize="40%"
              minSize="30%"
              collapsible
            >
              <Group orientation="vertical">
                {/* Основная информация */}
                <Panel id="info-panel"
                  defaultSize="35%"
                  minSize="30%"
                  className="p-2"
                  collapsible
                >
                  <Card className="flex flex-col h-full">
                    <CardHeader>
                      <CardTitle>Основная информация</CardTitle>
                    </CardHeader>
                    <CardContent className="flex flex-col gap-4">
                      <div className="grid gap-2">
                        <Label htmlFor="name">Имя</Label>
                        <Input id="name" defaultValue={character.name} />
                      </div>
                    </CardContent>
                    <CardFooter className="justify-end">
                      <Button>Сохранить</Button>
                    </CardFooter>
                  </Card>
                </Panel>

                <Separator withHandle />

                {/* Класс */}
                <Panel
                  id="class-panel"
                  defaultSize="65%"
                  minSize="40%"
                  className="p-2 flex-1"
                  collapsible
                >
                  <div className="h-full flex flex-col">
                    <CharacterClassSelector
                      selectedClassId={character.classId}
                      onSelectClass={() => { }}
                    />
                  </div>
                </Panel>
              </Group>
            </Panel>

            <Separator withHandle />

            {/* Правая часть — код */}
            <Panel
              id="code-panel"
              defaultSize="60%"
              minSize="30%"
              className="p-2"
              collapsible
            >
              <div className="h-full overflow-auto">
                <CharacterCodeBlock characterId={id!} />
              </div>
            </Panel>
          </Group>
        )}
      </LoaderState>
    </div>
  )
}