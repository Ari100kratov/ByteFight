import { useEffect, useState } from 'react'
import { useParams } from 'react-router-dom'
import { toast } from 'sonner'
import { RotateCcw } from 'lucide-react'

import { Button } from '@/components/ui/button'
import { Card, CardContent, CardFooter, CardHeader, CardTitle } from '@/components/ui/card'
import { Input } from '@/components/ui/input'
import { Label } from '@/components/ui/label'
import { Skeleton } from '@/components/ui/skeleton'
import { Spinner } from '@/components/ui/spinner'
import { useBreadcrumbNames } from '@/layouts/BreadcrumbProvider'
import { LoaderState } from '@/components/common/LoaderState'
import { useCharacter } from './hooks/useCharacter'
import { useRenameCharacter } from './hooks/useRenameCharacter'
import { CharacterClassSelector } from '../character-class-selector/CharacterClassSelector'
import { TalentsTree } from '../character-talents/TalentsTree'

interface CharacterNameDraft {
  characterId: string
  value: string
}

function ignoreCharacterClassSelection() {
  return undefined
}

function CharacterPageSkeleton() {
  return (
    <div className="flex h-full gap-4">
      <Skeleton className="h-full w-2/5 rounded-2xl" />
      <Skeleton className="h-full flex-1 rounded-2xl" />
    </div>
  )
}

/**
 * Страница героя: имя, класс со стезями и дерево талантов.
 * Редактор кода скрыт — героем управляют руками в бою.
 */
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

  useEffect(() => {
    if (!character) return

    setName(`/characters/${character.id}`, character.name)
  }, [character, setName])

  const savedName = character
    ? savedNameDraft?.characterId === character.id
      ? savedNameDraft.value
      : character.name
    : ''
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

      toast.success('Имя персонажа сохранено')
    } catch (renameErr) {
      toast.error(renameErr instanceof Error ? renameErr.message : 'Не удалось сохранить имя персонажа')
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
          <div className="flex h-full min-h-0 gap-4">
            <div className="flex h-full min-h-0 w-2/5 flex-col gap-4">
              <Card className="shrink-0 border-[#2c3a24] bg-[#161f12]/80">
                <CardHeader>
                  <CardTitle className="text-[#e8d9a0]">Основная информация</CardTitle>
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
                        'Сохранить'
                      )}
                    </Button>
                  </CardFooter>
                )}
              </Card>

              <Card className="min-h-0 flex-1 overflow-hidden border-[#2c3a24] bg-[#161f12]/80">
                <div className="h-full overflow-auto">
                  <CharacterClassSelector
                    selectedClassId={character.classId}
                    selectedSpecId={character.specId}
                    onSelectClass={ignoreCharacterClassSelection}
                    onSelectSpec={ignoreCharacterClassSelection}
                  />
                </div>
              </Card>
            </div>

            <Card className="min-h-0 flex-1 border-[#2c3a24] bg-[#161f12]/80 p-4">
              <TalentsTree characterId={character.id} />
            </Card>
          </div>
        )}
      </LoaderState>
    </div>
  )
}
