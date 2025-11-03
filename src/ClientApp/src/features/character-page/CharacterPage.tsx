import { useEffect } from "react"
import { useParams } from "react-router-dom"
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

export default function CharacterPage() {
  const { id } = useParams<{ id: string }>()
  const { data: character, isLoading, error } = useCharacter(id)
  const { setName } = useBreadcrumbNames()

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
          <div className="grid grid-cols-1 md:grid-cols-2 gap-6 w-full h-full">
            <div className="flex flex-col gap-6">
              <Skeleton className="h-40 rounded-2xl w-full" />
              <Skeleton className="h-40 rounded-2xl w-full" />
            </div>
            <Skeleton className="h-full rounded-2xl w-full" />
          </div>
        }
      >
        {character && (
          <div className="grid grid-cols-1 md:grid-cols-7 gap-6 w-full h-full">
            {/* Левая часть: Основная информация + Класс */}
            <div className="md:col-span-3 flex flex-col gap-6">
              {/* Основная информация */}
              <Card className="flex-1">
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

              {/* Класс */}
              <CharacterClassSelector selectedClassId={character.classId} onSelectClass={() => { }} />
            </div>

            {/* Правая часть: Код */}
            <div className="md:col-span-4 flex flex-col h-full">
              <CharacterCodeBlock characterId={id!} />
            </div>
          </div>
        )}
      </LoaderState>
    </div>
  )
}
