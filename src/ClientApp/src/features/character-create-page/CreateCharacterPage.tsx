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
import {
  ResizableHandle,
  ResizablePanel,
  ResizablePanelGroup,
} from "@/components/ui/resizable"

export default function CreateCharacterPage() {
  const [name, setName] = useState("")
  const [selectedClassId, setSelectedClassId] = useState<string>()

  const navigate = useNavigate()
  const { mutateAsync: create, isPending, error } = useCreateCharacter()

  const createPageLayout = useDefaultLayout({
    id: "create-character-layout",
    panelIds: ["create-character-main-info", "create-character-class"],
  })

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
      className="flex flex-col gap-6 p-4 w-full mx-auto h-full"
    >
      <ResizablePanelGroup
        id="create-character-layout"
        direction="horizontal"
        defaultLayout={createPageLayout.defaultLayout}
        onLayoutChanged={createPageLayout.onLayoutChanged}
        resizeTargetMinimumSize={{ coarse: 36, fine: 24 }}
        className="h-full rounded-2xl border"
      >
        <ResizablePanel id="create-character-main-info" defaultSize={35} minSize={25}>
          <Card className="flex h-full flex-col rounded-none border-0 border-r">
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
        </ResizablePanel>

        <ResizableHandle withHandle />

        <ResizablePanel id="create-character-class" defaultSize={65} minSize={35}>
          <div className="h-full overflow-auto">
            <CharacterClassSelector
              selectedClassId={selectedClassId}
              onSelectClass={setSelectedClassId}
            />
          </div>
        </ResizablePanel>
      </ResizablePanelGroup>

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
