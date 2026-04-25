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
import { SpriteAnimationPlayer } from "@/features/character-class-selector/components/SpriteAnimationPlayer"
import { CharacterStats } from "@/features/character-class-selector/components/CharacterStats"
import { StatType } from "@/shared/types/stat"
import { useArenaStore } from "@/features/game/state/data/arena.data.store"
import { useCharacterSelectionState } from "./hooks/useCharacterSelectionState"
import { useSelectedCharacterId } from "./hooks/useSelectedCharacterId"
import { CharacterIdentity } from "@/features/characters/components/CharacterIdentity"

export function SelectCharacterCard() {
  const arena = useArenaStore((s) => s.arena)
  const { data: characters, isLoading, error } = useCharacters()

  const { sessionCharacterId, isCharacterSelectionDisabled } =
    useCharacterSelectionState()

  const { selectedCharacterId, setSelectedCharacterId } = useSelectedCharacterId({
    characters,
    sessionCharacterId,
  })

  const { data: character } = useCharacterDetails(selectedCharacterId)
  const init = useCharacterStateStore((s) => s.init)

  useEffect(() => {
    if (!character || !arena) return

    const health = character.spec.stats.find(
      (s) => s.statType === StatType.Health
    )?.value

    const mana = character.spec.stats.find(
      (s) => s.statType === StatType.Mana
    )?.value

    init({
      characterId: character.id,
      maxHp: health ?? 0,
      maxMp: mana,
      startPosition: arena.startPosition,
    })
  }, [character?.id, arena?.startPosition, init])

  const handleCreateClick = () => {
    window.open("/characters/create", "_blank")
  }

  const hasCharacters = Boolean(characters && characters.length > 0)

  return (
    <LoaderState
      isLoading={isLoading}
      error={error}
      skeletonClassName="w-full h-full rounded-2xl"
      loadingFallback={<Skeleton className="w-full h-full rounded-2xl" />}
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

                  {characters!.map((char) => (
                    <SelectItem
                      key={char.id}
                      value={char.id}
                      className="py-2"
                    >
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

        <CardContent className="flex flex-col md:flex-row gap-4">
          {!hasCharacters && (
            <div className="flex flex-1 items-center justify-center py-8">
              <Button
                onClick={handleCreateClick}
                className="gap-2"
                disabled={isCharacterSelectionDisabled}
              >
                <Plus className="w-4 h-4" />
                Создать персонажа
              </Button>
            </div>
          )}

          {character && (
            <>
              <div className="flex items-center justify-center p-4">
                <SpriteAnimationPlayer actionAssets={character.spec.actionAssets} />
              </div>

              <CharacterStats stats={character.spec.stats} />
            </>
          )}
        </CardContent>
      </Card>
    </LoaderState>
  )
}