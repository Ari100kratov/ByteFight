import { Card, CardContent, CardFooter, CardHeader, CardTitle } from "@/components/ui/card"
import { Button } from "@/components/ui/button"
import { LoaderState } from "@/components/common/LoaderState"
import { useCharacterCodes } from "@/features/character-code-block/hooks/useCharacterCodes"
import { useCodeTemplate } from "@/features/character-code-block/hooks/useCodeTemplate"
import { useCodeEditor } from "./hooks/useCodeEditor"
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

  const { codes, activeCodeId, setActiveCode, addCode, markCodeDeleted, renameCode, updateCodeSource, resetToBaseline } =
    useCodeEditor(codesQuery, templateQuery)

  const hasChanges = codes.some(c => c.status !== ChangeStatus.Unchanged)

  const handleSave = () => {
    const created = codes
      .filter(c => c.status === ChangeStatus.Created)
      .map(({ id, name, sourceCode }) => ({ id, name, sourceCode }))
    const updated = codes
      .filter(c => c.status === ChangeStatus.Updated)
      .map(({ id, name, sourceCode }) => ({ id, name, sourceCode }))
    const deletedIds = codes
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
    <Card className={cn("flex flex-col w-full h-full overflow-auto min-w-[400px]", className)}>
      <CardHeader>
        <CardTitle>Поведение</CardTitle>
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
          {codes.length > 0 && (
            <CodeTabs
              codes={codes.filter(c => c.status !== "deleted")}
              activeTab={activeCodeId}
              onTabChange={setActiveCode}
              onAdd={addCode}
              onRename={renameCode}
              onDelete={markCodeDeleted}
              onChangeSource={updateCodeSource}
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
            onConfirm={resetToBaseline}
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
