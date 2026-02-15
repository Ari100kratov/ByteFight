import { useEffect } from "react"
import { useParams } from "react-router-dom"
import { Group, Panel, Separator, useDefaultLayout } from "react-resizable-panels"
import { GripVerticalIcon } from "lucide-react"
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

export default function CharacterPage() {
  const { id } = useParams<{ id: string }>()
  const { data: character, isLoading, error } = useCharacter(id)
  const { setName } = useBreadcrumbNames()

  const pageLayout = useDefaultLayout({
    id: "character-page-layout-v2",
    panelIds: ["character-page-left-column", "character-page-code"],
  })

  const leftColumnLayout = useDefaultLayout({
    id: "character-page-left-column-layout-v2",
    panelIds: ["character-page-main-info", "character-page-class"],
  })

  useEffect(() => {
    if (character) {
      setName(`/characters/${character.id}`, character.name)
    }
  }, [character, setName])

  return (
    <div className="flex flex-col gap-6 p-4 w-full h-full">
      <LoaderState
        isLoading={isLoading}
        error={error}
        skeletonClassName="w-full h-full rounded-2xl"
        loadingFallback={
          <Group
            id="character-page-loading-layout"
            orientation="horizontal"
            className="h-full w-full rounded-2xl border"
          >
            <Panel id="character-page-loading-left" defaultSize="40%" minSize="25%">
              <Group
                id="character-page-loading-left-column-layout"
                orientation="vertical"
                className="h-full w-full"
              >
                <Panel id="character-page-loading-main-info" defaultSize="45%" minSize="20%">
                  <Skeleton className="h-full w-full rounded-md" />
                </Panel>
                <ResizeHandle />
                <Panel id="character-page-loading-class" defaultSize="55%" minSize="30%">
                  <Skeleton className="h-full w-full rounded-md" />
                </Panel>
              </Group>
            </Panel>

            <ResizeHandle />

            <Panel id="character-page-loading-code" defaultSize="60%" minSize="35%">
              <Skeleton className="h-full w-full rounded-md" />
            </Panel>
          </Group>
        }
      >
        {character && (
          <Group
            id="character-page-layout-v2"
            orientation="horizontal"
            defaultLayout={pageLayout.defaultLayout}
            onLayoutChanged={pageLayout.onLayoutChanged}
            resizeTargetMinimumSize={{ coarse: 36, fine: 24 }}
            className="h-full w-full rounded-2xl border"
          >
            <Panel id="character-page-left-column" defaultSize="40%" minSize="25%">
              <Group
                id="character-page-left-column-layout-v2"
                orientation="vertical"
                defaultLayout={leftColumnLayout.defaultLayout}
                onLayoutChanged={leftColumnLayout.onLayoutChanged}
                resizeTargetMinimumSize={{ coarse: 36, fine: 24 }}
                className="h-full w-full"
              >
                <Panel id="character-page-main-info" defaultSize="40%" minSize="25%">
                  <Card className="flex h-full flex-col rounded-none border-0 border-b">
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

                <ResizeHandle />

                <Panel id="character-page-class" defaultSize="60%" minSize="30%">
                  <div className="h-full overflow-auto">
                    <CharacterClassSelector selectedClassId={character.classId} onSelectClass={() => { }} />
                  </div>
                </Panel>
              </Group>
            </Panel>

            <ResizeHandle />

            <Panel id="character-page-code" defaultSize="60%" minSize="35%">
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
