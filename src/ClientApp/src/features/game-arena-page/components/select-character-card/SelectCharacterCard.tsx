import { Card, CardHeader, CardTitle, CardContent } from "@/components/ui/card"
import { LoaderState } from "@/components/common/LoaderState"
import { Skeleton } from "@/components/ui/skeleton"
import {
  Select,
  SelectContent,
  SelectGroup,
  SelectItem,
  SelectLabel,
  SelectTrigger,
  SelectValue,
} from "@/components/ui/select"
import { Button } from "@/components/ui/button"
import { Plus } from "lucide-react"
import { useCharacters } from "@/features/characters-page/useCharacters"
import { useEffect } from "react"
import { useCharacterDetails } from "./hooks/useCharacterDetails"
import { useCharacterStateStore } from "@/features/game/state/game/character.state.store"
import { StatType } from "@/shared/types/stat"
import { useArenaStore } from "@/features/game/state/data/arena.data.store"
import { useCharacterSelectionState } from "./hooks/useCharacterSelectionState"
import { useSelectedCharacterId } from "./hooks/useSelectedCharacterId"
import { CharacterIdentity } from "@/features/characters/components/CharacterIdentity"
import { UnitPreview } from "@/features/unit-preview/UnitPreview"

export function SelectCharacterCard() {
  const arena = useArenaStore((s) => s.arena)
  const { data: characters, isLoading, error } = useCharacters()

  const { sessionCharacterId, isCharacterSelectionDisabled } = useCharacterSelectionState()

  const { selectedCharacterId, setSelectedCharacterId } = useSelectedCharacterId({
    characters,
    sessionCharacterId,
  })

  const { data: character } = useCharacterDetails(selectedCharacterId)
  const init = useCharacterStateStore((s) => s.init)

  useEffect(() => {
    if (!character || !arena) return

    const health = character.spec.stats.find((s) => s.statType === StatType.Health)?.value

    const mana = character.spec.stats.find((s) => s.statType === StatType.Mana)?.value

    init({
      characterId: character.id,
      maxHp: health ?? 0,
      maxMp: mana,
      startPosition: arena.startPosition,
    })
  }, [arena, character, init])

  const handleCreateClick = () => {
    window.open("/characters/create", "_blank")
  }

  const characterOptions = characters ?? []
  const hasCharacters = characterOptions.length > 0

  return (
    <LoaderState
      isLoading={isLoading}
      error={error}
      skeletonClassName="w-full h-full rounded-2xl"
      loadingFallback={<Skeleton className="h-full w-full rounded-2xl" />}
    >
      <Card className="h-full overflow-auto">
        <CardHeader>
          <CardTitle>Персонаж</CardTitle>

          {hasCharacters && (
            <Select
              value={selectedCharacterId}
              onValueChange={setSelectedCharacterId}
              disabled={isCharacterSelectionDisabled}
            >
              <SelectTrigger className="h-auto min-h-14 w-full items-start py-2 text-left">
                <SelectValue placeholder="Выберите персонажа" />
              </SelectTrigger>

              <SelectContent>
                <SelectGroup>
                  <SelectLabel>Ваши персонажи</SelectLabel>

                  {characterOptions.map((char) => (
                    <SelectItem key={char.id} value={char.id} className="py-2">
                      <CharacterIdentity
                        name={char.name}
                        className={char.className}
                        specName={char.specName}
                        size="sm"
                      />
                    </SelectItem>
                  ))}
                </SelectGroup>
              </SelectContent>
            </Select>
          )}
        </CardHeader>

        <CardContent className="flex flex-col gap-4 md:flex-row">
          {!hasCharacters && (
            <div className="flex flex-1 items-center justify-center py-8">
              <Button
                onClick={handleCreateClick}
                className="gap-2"
                disabled={isCharacterSelectionDisabled}
              >
                <Plus className="h-4 w-4" />
                Создать персонажа
              </Button>
            </div>
          )}

          {character && (
            <UnitPreview
              stats={character.spec.stats}
              actionAssets={character.spec.actionAssets}
              abilities={character.spec.abilities}
            />
          )}
        </CardContent>
      </Card>
    </LoaderState>
  )
}
