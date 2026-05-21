import { Card, CardContent, CardFooter, CardHeader, CardTitle } from "@/components/ui/card"
import { Button } from "@/components/ui/button"
import { LoaderState } from "@/components/common/LoaderState"
import { useCharacterCodes } from "@/features/character-code-block/hooks/useCharacterCodes"
import { useCodeTemplate } from "@/features/character-code-block/hooks/useCodeTemplate"
import { useCodeEditor } from "./hooks/useCodeEditor"
import { CodeTabs } from "./components/CodeTabs"
import { ChangeStatus } from "./types"
import { ConfirmDialog } from "@/components/common/ConfirmDialog"
import { BookOpen, Plus, RotateCcw } from "lucide-react"
import { Tooltip, TooltipContent, TooltipProvider, TooltipTrigger } from "@/components/ui/tooltip"
import { useUpdateCodes } from "./hooks/useUpdateCodes"
import { toast } from "sonner"
import { Spinner } from "@/components/ui/spinner"
import { cn } from "@/shared/lib/utils"
import { Link } from "react-router-dom"
import { getApiErrorToastMessage } from "@/shared/lib/apiErrors"

interface Props {
  characterId: string
  className?: string
}

export default function CharacterCodeBlock({ characterId, className }: Props) {
  const codesQuery = useCharacterCodes(characterId)
  const templateQuery = useCodeTemplate()
  const { mutate: updateCodes, isPending } = useUpdateCodes()

  const {
    codes,
    activeCodeId,
    setActiveCode,
    addCode,
    markCodeDeleted,
    renameCode,
    updateCodeSource,
    resetToBaseline,
  } = useCodeEditor(codesQuery, templateQuery)

  const hasChanges = codes.some((c) => c.status !== ChangeStatus.Unchanged)

  const handleAddCode = () => {
    void addCode()
  }

  const handleSave = () => {
    const created = codes
      .filter((c) => c.status === ChangeStatus.Created)
      .map(({ id, name, sourceCode }) => ({ id, name, sourceCode }))
    const updated = codes
      .filter((c) => c.status === ChangeStatus.Updated)
      .map(({ id, name, sourceCode }) => ({ id, name, sourceCode }))
    const deletedIds = codes.filter((c) => c.status === ChangeStatus.Deleted).map((c) => c.id)

    updateCodes(
      { characterId, created, updated, deletedIds },
      {
        onSuccess: () => {
          toast.success("Изменения успешно сохранены")
        },
        onError: (error: unknown) => {
          toast.error(`Ошибка при сохранении: ${getApiErrorToastMessage(error)}`)
        },
      },
    )
  }

  return (
    <Card className={cn("flex h-full w-full min-w-[400px] flex-col overflow-auto", className)}>
      <CardHeader className="flex flex-row items-start justify-between space-y-0">
        <CardTitle>Поведение</CardTitle>

        <Link
          to="/docs"
          target="_blank"
          rel="noreferrer"
          className="text-muted-foreground hover:text-foreground inline-flex items-center gap-1 text-sm transition-colors"
        >
          <BookOpen size={16} />
          Документация
        </Link>
      </CardHeader>

      <CardContent className="flex flex-1 flex-col">
        <LoaderState
          isLoading={codesQuery.isLoading}
          error={codesQuery.error}
          isEmpty={codes.length === 0}
          empty={
            <div className="text-muted-foreground flex flex-col items-center gap-2">
              <Button onClick={handleAddCode}>
                <Plus /> Добавить
              </Button>
            </div>
          }
          skeletonClassName="w-full h-full rounded-md"
        >
          {codes.length > 0 && (
            <CodeTabs
              codes={codes.filter((c) => c.status !== "deleted")}
              activeTab={activeCodeId}
              onTabChange={setActiveCode}
              onAdd={handleAddCode}
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
              <button
                disabled={isPending}
                className="text-muted-foreground rounded p-2 hover:bg-gray-200 dark:hover:bg-gray-700"
              >
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
