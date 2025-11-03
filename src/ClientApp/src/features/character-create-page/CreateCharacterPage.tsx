import React, { useState } from "react"
import { useNavigate } from "react-router-dom"
import { Button } from "@/components/ui/button"
import {
  Card,
  CardContent,
  CardHeader,
  CardTitle,
} from "@/components/ui/card"
import { Input } from "@/components/ui/input"
import { Label } from "@/components/ui/label"
import { Spinner } from "@/components/ui/spinner"
import { toast } from "sonner"
import { useCreateCharacter } from "./useCreateCharacter"
import { CharacterClassSelector } from "../character-class-selector/CharacterClassSelector"

export default function CreateCharacterPage() {
  const [name, setName] = useState("")
  const [selectedClassId, setSelectedClassId] = useState<string>()

  const navigate = useNavigate()
  const { mutateAsync: create, isPending, error } = useCreateCharacter()

  async function handleSubmit(e: React.FormEvent) {
    e.preventDefault()

    if (!selectedClassId) {
      toast.error("Пожалуйста, выберите класс персонажа")
      return
    }

    create(
      { name, classId: selectedClassId },
      {
        onSuccess: (id) => {
          navigate(`/characters/${id}`)
          toast.success("Персонаж успешно создан")
        },
      }
    )
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

        <CharacterClassSelector
          selectedClassId={selectedClassId}
          onSelectClass={setSelectedClassId}
        />
      </div>

      <div className="flex justify-end">
        <Button type="submit" disabled={isPending}>
          {isPending ? (
            <>
              <Spinner /> Создаем...
            </>
          ) : (
            "Создать персонажа"
          )}
        </Button>
      </div>
    </form>
  )
}
