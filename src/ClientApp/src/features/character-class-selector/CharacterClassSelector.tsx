import {
  Card,
  CardContent,
  CardHeader,
  CardTitle,
  CardDescription,
} from "@/components/ui/card"
import { ToggleGroup, ToggleGroupItem } from "@/components/ui/toggle-group"
import { CharacterClassType, useCharacterClasses } from "./useCharacterClasses"
import { LoaderState } from "@/components/common/LoaderState"
import type { CharacterClassResponse } from "./useCharacterClasses"
import { SpriteAnimationPlayer } from "./components/SpriteAnimationPlayer"
import { CharacterStats } from "./components/CharacterStats"
import { Sword, Wand } from "lucide-react"
import type { JSX } from "react"

interface Props {
  selectedClassId?: string
  onSelectClass: (id: string) => void
}

export function CharacterClassSelector({ selectedClassId, onSelectClass }: Props) {
  const { data, isLoading, error } = useCharacterClasses()
  const selectedClass = data?.find((cls) => cls.id === selectedClassId)

  const iconMap: Record<CharacterClassType, JSX.Element> = {
    [CharacterClassType.Warrior]: <Sword />,
    [CharacterClassType.Mage]: <Wand />,
  }

  return (
    <LoaderState
      isLoading={isLoading}
      error={error}
      skeletonClassName="flex flex-col flex-[4]"
      empty={null}
    >
      <Card className="flex flex-col flex-[4]">
        <CardHeader className="flex flex-col gap-2">
          <div className="flex items-center justify-between gap-3">
            <CardTitle>Класс</CardTitle>
            {data && data.length > 0 && (
              <ToggleGroup
                type="single"
                value={selectedClassId}
                onValueChange={onSelectClass}
                className="flex flex-wrap gap-1"
              >
                {data.map((cls: CharacterClassResponse) => (
                  <ToggleGroupItem key={cls.id} value={cls.id} className="flex items-center gap-1">
                    {iconMap[cls.type]}
                    <span>{cls.name}</span>
                  </ToggleGroupItem>
                ))}
              </ToggleGroup>
            )}
          </div>

          {selectedClass?.description && (
            <CardDescription>{selectedClass.description}</CardDescription>
          )}
        </CardHeader>

        <CardContent className="flex flex-col md:flex-row gap-6">
          {selectedClass ? (
            <>
              {/* Левая часть — спрайт */}
              <div className="flex items-center justify-center p-4">
                <SpriteAnimationPlayer actionAssets={selectedClass.actionAssets} />
              </div>

              {/* Правая часть — характеристики */}
              <CharacterStats stats={selectedClass.stats} />
            </>
          ) : (
            <div className="flex-1 flex items-center justify-center text-muted-foreground">
              Выберите класс персонажа
            </div>
          )}
        </CardContent>
      </Card>
    </LoaderState>
  )
}
