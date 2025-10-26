import { Card, CardContent, CardFooter, CardHeader, CardTitle } from "@/components/ui/card"
import { Button } from "@/components/ui/button"
import { LoaderState } from "@/components/common/LoaderState"
import { useCharacterCodes } from "@/features/character-code-block/hooks/useCharacterCodes"
import { useCodeTemplate } from "@/features/character-code-block/hooks/useCodeTemplate"
import { useLocalCodes } from "./hooks/useLocalCodes"
import { CodeTabs } from "./components/CodeTabs"
import { ChangeStatus } from "./types"
import { ConfirmDialog } from "@/components/common/ConfirmDialog"
import { Plus, RotateCcw } from "lucide-react"
import { Tooltip, TooltipContent, TooltipProvider, TooltipTrigger } from "@/components/ui/tooltip"
import { useUpdateCodes } from "./hooks/useUpdateCodes"
import { toast } from "sonner"
import { Spinner } from "@/components/ui/spinner"
import { cn } from "@/shared/lib/utils"

type Props = {
  characterId: string
  className?: string
}

export default function CharacterCodeBlock({ characterId, className }: Props) {
  const codesQuery = useCharacterCodes(characterId)
  const templateQuery = useCodeTemplate()
  const { mutate: updateCodes, isPending } = useUpdateCodes()

  const { localCodes, activeTab, setActiveTab, addCode, deleteCode, renameCode, changeSource, resetChanges } =
    useLocalCodes(codesQuery, templateQuery)

  const hasChanges = localCodes.some(c => c.status !== ChangeStatus.Unchanged)

  const handleSave = () => {
    const created = localCodes
      .filter(c => c.status === ChangeStatus.Created)
      .map(({ id, name, sourceCode }) => ({ id, name, sourceCode }))
    const updated = localCodes
      .filter(c => c.status === ChangeStatus.Updated)
      .map(({ id, name, sourceCode }) => ({ id, name, sourceCode }))
    const deletedIds = localCodes
      .filter(c => c.status === ChangeStatus.Deleted)
      .map(c => c.id)

    updateCodes(
      { characterId, created, updated, deletedIds },
      {
        onSuccess: () => {
          toast.success("Изменения успешно сохранены")
        },
        onError: (error: any) => {
          toast.error(`Ошибка при сохранении: ${error.message ?? error}`)
        },
      }
    )
  }

  return (
    <Card className={cn("flex flex-col w-full h-full", className)}>
      <CardHeader>
        <CardTitle>Поведение</CardTitle>
        {/* <CardDescription>Придумать описание</CardDescription> */}
      </CardHeader>

      <CardContent className="flex-1 flex flex-col">
        <LoaderState
          isLoading={codesQuery.isLoading}
          error={codesQuery.error}
          empty={
            <div className="flex flex-col items-center gap-2 text-muted-foreground">
              <Button onClick={addCode}><Plus /> Добавить</Button>
            </div>
          }
          skeletonClassName="w-full h-full rounded-md"
        >
          {localCodes.length > 0 && (
            <CodeTabs
              codes={localCodes.filter(c => c.status !== "deleted")}
              activeTab={activeTab}
              onTabChange={setActiveTab}
              onAdd={addCode}
              onRename={renameCode}
              onDelete={deleteCode}
              onChangeSource={changeSource}
            />
          )}
        </LoaderState>
      </CardContent>

      {hasChanges && (
        <CardFooter className="justify-end gap-2">
          <ConfirmDialog
            trigger={
              <button disabled={isPending} className="p-2 rounded hover:bg-gray-200 dark:hover:bg-gray-700 text-muted-foreground">
                <TooltipProvider>
                  <Tooltip>
                    <TooltipTrigger asChild>
                      <span>
                        <RotateCcw size={18} />
                      </span>
                    </TooltipTrigger>
                    <TooltipContent>Отменить изменения</TooltipContent>
                  </Tooltip>
                </TooltipProvider>
              </button>
            }
            title="Отменить изменения?"
            description="Все несохранённые изменения будут потеряны."
            onConfirm={resetChanges}
          />
          <Button onClick={handleSave} disabled={isPending}>
            {isPending ? (
              <>
                <Spinner /> Сохраняем...
              </>
            ) : (
              "Сохранить"
            )}
          </Button>
        </CardFooter>
      )}
    </Card>
  )
}
