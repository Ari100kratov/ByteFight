import { useEffect } from "react"
import { useParams } from "react-router-dom"
import { Button } from "@/components/ui/button"
import {
  Card,
  CardContent,
  CardDescription,
  CardFooter,
  CardHeader,
  CardTitle,
} from "@/components/ui/card"
import { Input } from "@/components/ui/input"
import { Label } from "@/components/ui/label"
import { useCharacter } from "@/hooks/characters/useCharacter"
import { Skeleton } from "@/components/ui/skeleton"
import { Alert, AlertDescription, AlertTitle } from "@/components/ui/alert"
import { useBreadcrumbNames } from "@/layouts/BreadcrumbProvider"
import CharacterCodeBlock from "@/components/characters/CharacterCodeBlock"

export default function CharacterDetailsPage() {
  const { id } = useParams<{ id: string }>()
  const { data: character, isLoading, error } = useCharacter(id)
  const { setName } = useBreadcrumbNames()

  useEffect(() => {
    if (character) {
      setName(`/characters/${character.id}`, character.name)
    }
  }, [character, setName])

  if (isLoading) {
    return (
      <div className="flex flex-col gap-6 p-4 w-full h-full">
        <div className="grid grid-cols-1 md:grid-cols-2 gap-6 w-full">
          <Skeleton className="h-40 rounded-2xl w-full" />
          <Skeleton className="h-40 rounded-2xl w-full" />
        </div>
        <div className="grid grid-cols-1 md:grid-cols-3 gap-6 w-full">
          <Skeleton className="h-60 rounded-2xl w-full md:col-span-2" />
          <Skeleton className="h-60 rounded-2xl w-full" />
        </div>
      </div>
    )
  }

  if (error)
    return (
      <Alert variant="destructive">
        <AlertTitle>Ошибка</AlertTitle>
        <AlertDescription>{error.message}</AlertDescription>
      </Alert>
    )

  if (!character) return null

  return (
    <div className="flex flex-col gap-6 p-4 w-full h-full">
      {/* 1 строка: Основная информация и Класс */}
      <div className="grid grid-cols-1 md:grid-cols-2 gap-6 w-full">
        {/* Основная информация */}
        <Card className="w-full">
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
        <Card className="w-full">
          <CardHeader>
            <CardTitle>Класс</CardTitle>
            <CardDescription>Пока выбор недоступен</CardDescription>
          </CardHeader>
          <CardContent className="h-32 flex items-center justify-center text-muted-foreground">
            Блок выбора класса
          </CardContent>
        </Card>
      </div>

      {/* 2 строка: Код + Статистика */}
      <div className="grid grid-cols-1 md:grid-cols-3 gap-6 w-full h-full">
        {/* Код */}
        <CharacterCodeBlock characterId={id!} />

        {/* Статистика */}
        <Card className="w-full">
          <CardHeader>
            <CardTitle>Статистика</CardTitle>
          </CardHeader>
          <CardContent className="space-y-2">
            <p>Уровень: 10</p>
            <p>Опыт: 12512</p>
            <p>Победы: 12512512</p>
            <p>Поражения: 125123</p>
          </CardContent>
        </Card>
      </div>
    </div>
  )
}
