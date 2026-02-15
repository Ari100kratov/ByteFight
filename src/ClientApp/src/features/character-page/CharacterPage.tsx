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
import {
  ResizableHandle,
  ResizablePanel,
  ResizablePanelGroup,
} from "@/components/ui/resizable"

export default function CharacterPage() {
  const { id } = useParams<{ id: string }>()
  const { data: character, isLoading, error } = useCharacter(id)
  const { setName } = useBreadcrumbNames()

  const pageLayout = useDefaultLayout({
    id: "character-page-layout",
    panelIds: ["character-page-left-column", "character-page-code"],
  })

  const leftColumnLayout = useDefaultLayout({
    id: "character-page-left-column-layout",
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
          <ResizablePanelGroup
            id="character-page-loading-layout"
            direction="horizontal"
            className="h-full rounded-2xl border"
          >
            <ResizablePanel id="character-page-loading-left" defaultSize={40} minSize={25}>
              <ResizablePanelGroup
                id="character-page-loading-left-column-layout"
                direction="vertical"
                className="h-full"
              >
                <ResizablePanel id="character-page-loading-main-info" defaultSize={45} minSize={20}>
                  <Skeleton className="h-full w-full rounded-md" />
                </ResizablePanel>
                <ResizableHandle withHandle />
                <ResizablePanel id="character-page-loading-class" defaultSize={55} minSize={30}>
                  <Skeleton className="h-full w-full rounded-md" />
                </ResizablePanel>
              </ResizablePanelGroup>
            </ResizablePanel>

            <ResizableHandle withHandle />

            <ResizablePanel id="character-page-loading-code" defaultSize={60} minSize={35}>
              <Skeleton className="h-full w-full rounded-md" />
            </ResizablePanel>
          </ResizablePanelGroup>
        }
      >
        {character && (
          <ResizablePanelGroup
            id="character-page-layout"
            direction="horizontal"
            defaultLayout={pageLayout.defaultLayout}
            onLayoutChanged={pageLayout.onLayoutChanged}
            resizeTargetMinimumSize={{ coarse: 36, fine: 24 }}
            className="h-full rounded-2xl border"
          >
            <ResizablePanel id="character-page-left-column" defaultSize={40} minSize={25}>
              <ResizablePanelGroup
                id="character-page-left-column-layout"
                direction="vertical"
                defaultLayout={leftColumnLayout.defaultLayout}
                onLayoutChanged={leftColumnLayout.onLayoutChanged}
                resizeTargetMinimumSize={{ coarse: 36, fine: 24 }}
                className="h-full"
              >
                <ResizablePanel id="character-page-main-info" defaultSize={40} minSize={25}>
                  <Card className="flex-1 h-full rounded-none border-0 border-b">
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
                </ResizablePanel>

                <ResizableHandle withHandle />

                <ResizablePanel id="character-page-class" defaultSize={60} minSize={30}>
                  <div className="h-full overflow-auto">
                    <CharacterClassSelector selectedClassId={character.classId} onSelectClass={() => { }} />
                  </div>
                </ResizablePanel>
              </ResizablePanelGroup>
            </ResizablePanel>

            <ResizableHandle withHandle />

            <ResizablePanel id="character-page-code" defaultSize={60} minSize={35}>
              <div className="h-full overflow-auto">
                <CharacterCodeBlock characterId={id!} />
              </div>
            </ResizablePanel>
          </ResizablePanelGroup>
        )}
      </LoaderState>
    </div>
  )
}
