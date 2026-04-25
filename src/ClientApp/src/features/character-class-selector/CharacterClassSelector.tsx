import {
  Card,
  CardContent,
  CardDescription,
  CardHeader,
  CardTitle,
} from "@/components/ui/card"
import { ToggleGroup, ToggleGroupItem } from "@/components/ui/toggle-group"
import { LoaderState } from "@/components/common/LoaderState"
import { CharacterClassType, useCharacterClasses } from "./hooks/useCharacterClasses"
import type { CharacterClassResponse } from "./hooks/useCharacterClasses"
import { SpriteAnimationPlayer } from "./components/SpriteAnimationPlayer"
import { CharacterStats } from "./components/CharacterStats"
import { Sword, Wand } from "lucide-react"
import type { JSX } from "react"
import {
  useCharacterSpecsByClassId,
  type CharacterSpecResponse,
} from "./hooks/useCharacterSpecs"

interface Props {
  selectedClassId?: string
  selectedSpecId?: string
  onSelectClass: (id: string) => void
  onSelectSpec: (id: string) => void
}

export function CharacterClassSelector({
  selectedClassId,
  selectedSpecId,
  onSelectClass,
  onSelectSpec,
}: Props) {
  const {
    data: classes,
    isLoading: isClassesLoading,
    error: classesError,
  } = useCharacterClasses()

  const {
    data: specs,
    isLoading: isSpecsLoading,
    error: specsError,
  } = useCharacterSpecsByClassId(selectedClassId)

  const selectedClass = classes?.find((cls) => cls.id === selectedClassId)
  const selectedSpec = specs?.find((spec) => spec.id === selectedSpecId)

  const iconMap: Record<CharacterClassType, JSX.Element> = {
    [CharacterClassType.Warrior]: <Sword className="size-4" />,
    [CharacterClassType.Mage]: <Wand className="size-4" />,
  }

  function handleSelectClass(classId: string) {
    if (!classId || classId === selectedClassId) {
      return
    }

    onSelectClass(classId)
    onSelectSpec("")
  }

  function handleSelectSpec(specId: string) {
    if (!specId || specId === selectedSpecId) {
      return
    }

    onSelectSpec(specId)
  }

  return (
    <LoaderState
      isLoading={isClassesLoading}
      error={classesError}
      skeletonClassName="flex flex-col flex-[4]"
      empty={null}
    >
      <Card className="flex flex-col flex-[4]">
        <CardHeader>
          <CardTitle>Персонаж</CardTitle>
        </CardHeader>

        <CardContent className="flex flex-col gap-6">
          {classes && classes.length > 0 && (
            <section className="space-y-3">
              <h3 className="text-sm font-medium">Класс</h3>

              <ToggleGroup
                type="single"
                value={selectedClassId}
                onValueChange={handleSelectClass}
                className="grid grid-cols-1 sm:grid-cols-2 gap-3"
              >
                {classes.map((cls: CharacterClassResponse) => (
                  <ToggleGroupItem
                    key={cls.id}
                    value={cls.id}
                    className="h-auto justify-start gap-3 rounded-lg border p-4 text-left"
                  >
                    {iconMap[cls.type]}
                    <span className="font-medium">{cls.name}</span>
                  </ToggleGroupItem>
                ))}
              </ToggleGroup>

              {selectedClass?.description && (
                <p className="text-sm text-muted-foreground">
                  {selectedClass.description}
                </p>
              )}
            </section>
          )}

          {selectedClass ? (
            <LoaderState
              isLoading={isSpecsLoading}
              error={specsError}
              skeletonClassName="min-h-32"
              empty={null}
            >
              <section className="space-y-3">
                <h3 className="text-sm font-medium">Специализация</h3>

                {specs && specs.length > 0 ? (
                  <ToggleGroup
                    type="single"
                    value={selectedSpecId}
                    onValueChange={handleSelectSpec}
                    className="grid grid-cols-1 md:grid-cols-3 gap-3"
                  >
                    {specs.map((spec: CharacterSpecResponse) => (
                      <ToggleGroupItem
                        key={spec.id}
                        value={spec.id}
                        className="h-auto justify-start rounded-lg border p-4 text-left"
                      >
                        <span className="font-medium">{spec.name}</span>
                      </ToggleGroupItem>
                    ))}
                  </ToggleGroup>
                ) : (
                  <div className="text-sm text-muted-foreground">
                    Для выбранного класса пока нет специализаций
                  </div>
                )}

                {selectedSpec ? (
                  <div className="flex flex-col gap-4">
                    <div>
                      {selectedSpec.description && (
                        <p className="mt-1 text-sm text-muted-foreground">
                          {selectedSpec.description}
                        </p>
                      )}
                    </div>

                    <div className="flex flex-col md:flex-row gap-6">
                      <div className="flex items-center justify-center p-4">
                        <SpriteAnimationPlayer actionAssets={selectedSpec.actionAssets} />
                      </div>

                      <CharacterStats stats={selectedSpec.stats} />
                    </div>
                  </div>
                ) : (
                  <div className="min-h-40 flex items-center justify-center text-muted-foreground">
                    Выберите специализацию персонажа
                  </div>
                )}
              </section>
            </LoaderState>
          ) : (
            <div className="min-h-40 flex items-center justify-center text-muted-foreground">
              Выберите класс персонажа
            </div>
          )}

        </CardContent>
      </Card>
    </LoaderState>
  )
}