import React, { useState } from "react"
import { useNavigate } from "react-router-dom"
import { Group, Panel, Separator, useDefaultLayout } from "react-resizable-panels"
import { GripVerticalIcon } from "lucide-react"
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
import { cn } from "@/shared/lib/utils"

function ResizeHandle() {
  return (
    <Separator
      className={cn(
        "bg-border focus-visible:ring-ring relative flex w-px items-center justify-center after:absolute after:inset-y-0 after:left-1/2 after:w-1 after:-translate-x-1/2 focus-visible:ring-1 focus-visible:ring-offset-1 focus-visible:outline-hidden aria-[orientation=vertical]:h-px aria-[orientation=vertical]:w-full aria-[orientation=vertical]:after:left-0 aria-[orientation=vertical]:after:h-1 aria-[orientation=vertical]:after:w-full aria-[orientation=vertical]:after:translate-x-0 aria-[orientation=vertical]:after:-translate-y-1/2 [&[aria-orientation=vertical]>div]:rotate-90"
      )}
    >
      <div className="bg-border z-10 flex h-4 w-3 items-center justify-center rounded-xs border">
        <GripVerticalIcon className="size-2.5" />
      </div>
    </Separator>
  )
}

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
      <Group
        id="create-character-layout"
        orientation="horizontal"
        defaultLayout={createPageLayout.defaultLayout}
        onLayoutChanged={createPageLayout.onLayoutChanged}
        resizeTargetMinimumSize={{ coarse: 36, fine: 24 }}
        className="h-full w-full rounded-2xl border"
      >
        <Panel id="create-character-main-info" defaultSize={35} minSize={25}>
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
        </Panel>

        <ResizeHandle />

        <Panel id="create-character-class" defaultSize={65} minSize={35}>
          <div className="h-full overflow-auto">
            <CharacterClassSelector
              selectedClassId={selectedClassId}
              onSelectClass={setSelectedClassId}
            />
          </div>
        </Panel>
      </Group>

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
