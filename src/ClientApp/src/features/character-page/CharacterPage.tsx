import { useEffect, useState } from "react"
import { useParams } from "react-router-dom"
import { useDefaultLayout } from "react-resizable-panels"
import { toast } from "sonner"
import { RotateCcw } from "lucide-react"

import { Button } from "@/components/ui/button"
import { Card, CardContent, CardFooter, CardHeader, CardTitle } from "@/components/ui/card"
import { Input } from "@/components/ui/input"
import { Label } from "@/components/ui/label"
import { Skeleton } from "@/components/ui/skeleton"
import { Spinner } from "@/components/ui/spinner"
import { useBreadcrumbNames } from "@/layouts/BreadcrumbProvider"
import CharacterCodeBlock from "@/features/character-code-block/CharacterCodeBlock"
import { LoaderState } from "@/components/common/LoaderState"
import { useCharacter } from "./hooks/useCharacter"
import { useRenameCharacter } from "./hooks/useRenameCharacter"
import { CharacterClassSelector } from "../character-class-selector/CharacterClassSelector"
import { Group, Panel, Separator } from "@/components/ui/resizable"

interface CharacterNameDraft {
  characterId: string
  value: string
}

function ignoreCharacterClassSelection() {
  return undefined
}

function CharacterPageSkeleton() {
  return (
    <Group orientation="horizontal">
      <Panel id="left-panel-skeleton" defaultSize="40%" minSize="30%">
        <Group orientation="vertical">
          <Panel id="info-panel-skeleton" defaultSize="35%" minSize="30%" className="p-2">
            <Skeleton className="h-full w-full rounded-md" />
          </Panel>

          <Separator withHandle />

          <Panel id="class-panel-skeleton" defaultSize="65%" minSize="40%" className="p-2">
            <Skeleton className="h-full w-full rounded-md" />
          </Panel>
        </Group>
      </Panel>

      <Separator withHandle />

      <Panel id="code-panel-skeleton" defaultSize="60%" minSize="30%" className="p-2">
        <Skeleton className="h-full w-full rounded-md" />
      </Panel>
    </Group>
  )
}

export default function CharacterPage() {
  const { id } = useParams<{ id: string }>()
  const { data: character, isLoading, error } = useCharacter(id)
  const {
    mutateAsync: renameCharacter,
    isPending: isRenaming,
    error: renameError,
  } = useRenameCharacter()
  const { setName } = useBreadcrumbNames()

  const [nameDraft, setNameDraft] = useState<CharacterNameDraft | null>(null)
  const [savedNameDraft, setSavedNameDraft] = useState<CharacterNameDraft | null>(null)

  const { defaultLayout: rootDefaultLayout, onLayoutChanged: onRootLayoutChanged } =
    useDefaultLayout({ id: "character-layout" })

  const { defaultLayout: leftDefaultLayout, onLayoutChanged: onLeftLayoutChanged } =
    useDefaultLayout({ id: "character-left-layout" })

  useEffect(() => {
    if (!character) return

    setName(`/characters/${character.id}`, character.name)
  }, [character, setName])

  const savedName = character
    ? savedNameDraft?.characterId === character.id
      ? savedNameDraft.value
      : character.name
    : ""
  const name = character && nameDraft?.characterId === character.id ? nameDraft.value : savedName
  const trimmedName = name.trim()
  const isNameChanged = !!character && trimmedName !== savedName
  const canSaveName = isNameChanged && trimmedName.length > 0 && !isRenaming

  function updateNameDraft(value: string) {
    if (!character) return

    setNameDraft({
      characterId: character.id,
      value,
    })
  }

  async function handleSaveName() {
    if (!character || !canSaveName) return

    try {
      await renameCharacter({
        id: character.id,
        name: trimmedName,
      })

      const nextNameDraft = {
        characterId: character.id,
        value: trimmedName,
      }

      setSavedNameDraft(nextNameDraft)
      setNameDraft(nextNameDraft)
      setName(`/characters/${character.id}`, trimmedName)

      toast.success("Имя персонажа сохранено")
    } catch (error) {
      toast.error(error instanceof Error ? error.message : "Не удалось сохранить имя персонажа")
    }
  }

  return (
    <div className="flex h-full w-full flex-col gap-4">
      <LoaderState
        isLoading={isLoading}
        error={error}
        skeletonClassName="w-full h-full rounded-2xl"
        loadingFallback={<CharacterPageSkeleton />}
      >
        {character && (
          <Group
            orientation="horizontal"
            defaultLayout={rootDefaultLayout}
            onLayoutChanged={onRootLayoutChanged}
          >
            <Panel id="left-panel" defaultSize="40%" minSize="30%" collapsible>
              <Group
                orientation="vertical"
                defaultLayout={leftDefaultLayout}
                onLayoutChanged={onLeftLayoutChanged}
              >
                <Panel id="info-panel" defaultSize="35%" minSize="30%" className="p-2" collapsible>
                  <Card className="flex h-full flex-col overflow-auto">
                    <CardHeader>
                      <CardTitle>Основная информация</CardTitle>
                    </CardHeader>

                    <CardContent className="flex flex-col gap-4">
                      <div className="grid gap-2">
                        <Label htmlFor="name">Имя</Label>
                        <Input
                          id="name"
                          value={name}
                          maxLength={32}
                          onChange={(e) => {
                            updateNameDraft(e.target.value)
                          }}
                        />
                      </div>
                      {renameError && <p className="text-sm text-red-500">{renameError.message}</p>}
                    </CardContent>

                    {isNameChanged && (
                      <CardFooter className="justify-end gap-2">
                        <Button
                          type="button"
                          variant="ghost"
                          size="icon"
                          disabled={isRenaming}
                          onClick={() => {
                            updateNameDraft(savedName)
                          }}
                          title="Отменить изменения"
                        >
                          <RotateCcw className="size-4" />
                        </Button>

                        <Button
                          type="button"
                          onClick={() => {
                            void handleSaveName()
                          }}
                          disabled={!canSaveName}
                        >
                          {isRenaming ? (
                            <>
                              <Spinner /> Сохраняем...
                            </>
                          ) : (
                            "Сохранить"
                          )}
                        </Button>
                      </CardFooter>
                    )}
                  </Card>
                </Panel>

                <Separator withHandle />

                <Panel
                  id="class-panel"
                  defaultSize="65%"
                  minSize="40%"
                  className="flex-1 p-2"
                  collapsible
                >
                  <div className="flex h-full flex-col">
                    <CharacterClassSelector
                      selectedClassId={character.classId}
                      selectedSpecId={character.specId}
                      onSelectClass={ignoreCharacterClassSelection}
                      onSelectSpec={ignoreCharacterClassSelection}
                    />
                  </div>
                </Panel>
              </Group>
            </Panel>

            <Separator withHandle />

            <Panel id="code-panel" defaultSize="60%" minSize="30%" className="p-2" collapsible>
              <div className="h-full overflow-auto">
                <CharacterCodeBlock characterId={character.id} />
              </div>
            </Panel>
          </Group>
        )}
      </LoaderState>
    </div>
  )
}
