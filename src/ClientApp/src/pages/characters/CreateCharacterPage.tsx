import React, { useState } from "react"
import { useNavigate } from "react-router-dom"
import { Button } from "@/components/ui/button"
import {
  Card,
  CardContent,
  CardDescription,
  CardHeader,
  CardTitle,
} from "@/components/ui/card"
import { Input } from "@/components/ui/input"
import { Label } from "@/components/ui/label"
import { useCreateCharacter } from "@/hooks/characters/useCreateCharacter"

export default function CreateCharacterPage() {
  const [name, setName] = useState("")
  const navigate = useNavigate()
  const { mutateAsync: create, isPending, error } = useCreateCharacter()

  async function handleSubmit(e: React.FormEvent) {
    e.preventDefault()
    try {
      const id = await create({ name })
      navigate(`/characters/${id}`)
    } catch {
      // Ошибка уже показывается через error
    }
  }

  return (
    <form
      onSubmit={handleSubmit}
      className="flex flex-col gap-6 p-4 w-full mx-auto"
    >
      <div className="flex flex-col md:flex-row gap-6 w-full">
        <Card className="flex-[2]">
          <CardHeader>
            <CardTitle>Основная информация</CardTitle>
            <CardDescription>Введите имя вашего персонажа</CardDescription>
          </CardHeader>
          <CardContent className="flex flex-col gap-4">
            <div className="grid gap-2">
              <Label htmlFor="name">Имя</Label>
              <Input
                id="name"
                value={name}
                onChange={(e) => setName(e.target.value)}
              />
            </div>
            {error && <p className="text-sm text-red-500">{error.message}</p>}
          </CardContent>
        </Card>

        <Card className="flex-[4]">
          <CardHeader>
            <CardTitle>Класс персонажа</CardTitle>
            <CardDescription>
              Выберите класс вашего персонажа (пока пусто)
            </CardDescription>
          </CardHeader>
          <CardContent className="h-24 flex items-center justify-center text-muted-foreground">
            Блок выбора класса и описание будет здесь
          </CardContent>
        </Card>
      </div>

      <div className="flex justify-end">
        <Button type="submit" disabled={isPending}>
          {isPending ? "Создаём..." : "Создать персонажа"}
        </Button>
      </div>
    </form>
  )
}
