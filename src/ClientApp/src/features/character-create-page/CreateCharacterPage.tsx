import React, { useState } from "react"
import { useNavigate } from "react-router-dom"
import { useDefaultLayout } from "react-resizable-panels"

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
import { Group, Panel, Separator } from "@/components/ui/resizable"

export default function CreateCharacterPage() {
  const [name, setName] = useState("")
  const [selectedClassId, setSelectedClassId] = useState<string>()

  const navigate = useNavigate()
  const { mutateAsync: create, isPending, error } = useCreateCharacter()

  const { defaultLayout, onLayoutChanged } = useDefaultLayout({
    id: "create-character-layout"
  })

  async function handleSubmit(e: React.FormEvent) {
    e.preventDefault()

    if (!selectedClassId) {
      toast.error("Необходимо выбрать класс персонажа")
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
      className="flex flex-col w-full h-full"
    >
      <Group
        orientation="horizontal"
        defaultLayout={defaultLayout}
        onLayoutChanged={onLayoutChanged}
      >
        {/* Левая часть */}
        <Panel
          id="info-panel"
          defaultSize="40%"
          minSize="30%"
          className="p-2"
        >
          <Card className="h-full">
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
              {error && (
                <p className="text-sm text-red-500">
                  {error.message}
                </p>
              )}
            </CardContent>
          </Card>
        </Panel>

        <Separator withHandle />

        {/* Правая часть */}
        <Panel
          id="class-panel"
          defaultSize="60%"
          minSize="40%"
          className="p-2 flex flex-col"
        >
          <CharacterClassSelector
            selectedClassId={selectedClassId}
            onSelectClass={setSelectedClassId}
          />

          <div className="flex justify-end mt-auto pt-4">
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
        </Panel>
      </Group>
    </form>
  )
}