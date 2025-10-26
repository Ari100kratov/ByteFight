import {
  Card,
  CardContent,
  CardHeader,
  CardTitle,
} from "@/components/ui/card"
import { ToggleGroup, ToggleGroupItem } from "@/components/ui/toggle-group"
import { useCharacterClasses } from "./useCharacterClasses"
import type { CharacterClassResponse } from "./useCharacterClasses"
import { LoaderState } from "@/components/common/LoaderState"

interface CharacterClassSelectorProps {
  selectedClassId?: string
  onSelectClass: (id: string) => void
}

export function CharacterClassSelector({ selectedClassId, onSelectClass }: CharacterClassSelectorProps) {
  const { data, isLoading, error } = useCharacterClasses()

  return (
    <Card className="flex-[4]">
      <CardHeader>
        <CardTitle>Класс персонажа</CardTitle>
      </CardHeader>
      <CardContent>
        <LoaderState
          isLoading={isLoading}
          error={error}
          empty={<div className="text-sm text-muted-foreground">Классы пока не добавлены</div>}
        >
          <ToggleGroup
            type="single"
            value={selectedClassId}
            onValueChange={onSelectClass}
            className="flex flex-wrap gap-4"
          >
            {data?.map((cls: CharacterClassResponse) => (
              <ToggleGroupItem key={cls.id} value={cls.id}>
                {cls.name}
              </ToggleGroupItem>
            ))}
          </ToggleGroup>
        </LoaderState>
      </CardContent>
    </Card>
  )
}
