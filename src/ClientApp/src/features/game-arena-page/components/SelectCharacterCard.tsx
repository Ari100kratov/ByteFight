import { Card, CardHeader, CardTitle, CardContent, CardDescription } from "@/components/ui/card"
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
import { useEffect, useState } from "react"
import { useCharacterDetails } from "../hooks/useCharacterDetails"
import { useCharacterStateStore } from "@/features/game/state/game/character.state.store"
import { SpriteAnimationPlayer } from "@/features/character-class-selector/components/SpriteAnimationPlayer"
import { CharacterStats } from "@/features/character-class-selector/components/CharacterStats"
import { StatType } from "@/shared/types/stat"

export function SelectCharacterCard() {
  const { data: characters, isLoading, error } = useCharacters()
  const [selectedCharacterId, setSelectedCharacterId] = useState<string | undefined>()
  const { data: character } = useCharacterDetails(selectedCharacterId)
  const init = useCharacterStateStore(s => s.init)

  useEffect(() => {
    if (!character) return

    const health = character.class.stats.find(s => s.statType === StatType.Health)?.value
    const mana = character.class.stats.find(s => s.statType === StatType.Mana)?.value
    init({ characterId: character.id, maxHp: health ?? 0, maxMp: mana })

  }, [character?.id])

  const handleCreateClick = () => {
    window.open("/characters/create", "_blank")
  }

  const hasCharacters = characters && characters.length > 0

  return (
    <LoaderState
      isLoading={isLoading}
      error={error}
      skeletonClassName="w-full h-full rounded-2xl"
      loadingFallback={<Skeleton className="w-full h-full rounded-2xl" />}
    >
      <Card className="h-full rounded-none md:rounded-tl-2xl border-0 border-b md:border-b-0 md:border-r overflow-auto">
        <CardHeader>
          <CardTitle>Персонаж</CardTitle>
          <CardDescription>
            {hasCharacters && (
              <Select
                value={selectedCharacterId}
                onValueChange={setSelectedCharacterId}
              >
                <SelectTrigger className="w-full">
                  <SelectValue placeholder="Выберите персонажа" />
                </SelectTrigger>
                <SelectContent>
                  <SelectGroup>
                    <SelectLabel>Ваши персонажи</SelectLabel>
                    {characters.map((char) => (
                      <SelectItem key={char.id} value={char.id}>
                        {char.name}
                      </SelectItem>
                    ))}
                  </SelectGroup>
                </SelectContent>
              </Select>
            )}
          </CardDescription>
        </CardHeader>

        <CardContent className="flex flex-col md:flex-row gap-4">
          {!hasCharacters && (
            <div className="flex flex-1 items-center justify-center py-8">
              <Button onClick={handleCreateClick} className="gap-2">
                <Plus className="w-4 h-4" /> Создать персонажа
              </Button>
            </div>
          )}

          {character && (
            <>
              {/* Левая часть — спрайт */}
              <div className="flex items-center justify-center p-4">
                <SpriteAnimationPlayer actionAssets={character.class.actionAssets} />
              </div>

              {/* Правая часть — характеристики */}
              <CharacterStats stats={character.class.stats} />
            </>
          )}
        </CardContent>
      </Card>
    </LoaderState>
  )
}
