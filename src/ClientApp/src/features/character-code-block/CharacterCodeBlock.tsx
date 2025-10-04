import { Card, CardContent, CardDescription, CardFooter, CardHeader, CardTitle } from "@/components/ui/card"
import { Button } from "@/components/ui/button"
import { LoaderState } from "@/components/common/LoaderState"
import { useCharacterCodes } from "@/hooks/characters/codes/useCharacterCodes"
import { useCodeTemplate } from "@/hooks/characters/codes/useCodeTemplate"
import { useLocalCodes } from "./useLocalCodes"
import { CodeTabs } from "./CodeTabs"
import { ChangeStatus } from "./types"
import { ConfirmDialog } from "@/components/common/ConfirmDialog"
import { Plus, RotateCcw } from "lucide-react"
import { Tooltip, TooltipContent, TooltipProvider, TooltipTrigger } from "@/components/ui/tooltip"

type Props = {
  characterId: string
}

export default function CharacterCodeBlock({ characterId }: Props) {
  const codesQuery = useCharacterCodes(characterId)
  const templateQuery = useCodeTemplate()

  const { localCodes, activeTab, setActiveTab, addCode, deleteCode, renameCode, changeSource, resetChanges } =
    useLocalCodes(codesQuery, templateQuery)

  const hasChanges = localCodes.some(c => c.status !== ChangeStatus.Unchanged)

  const handleSave = async () => {
    console.log("Сохраняем все коды:", localCodes)
    // здесь логика вызова API
  }

  return (
    <Card className="flex flex-col w-full h-full">
      <CardHeader>
        <CardTitle>Код</CardTitle>
        <CardDescription>Придумать описание</CardDescription>
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
              <button className="p-2 rounded hover:bg-gray-200 dark:hover:bg-gray-700 text-muted-foreground">
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
          <Button onClick={handleSave}>Сохранить</Button>
        </CardFooter>
      )}
    </Card>
  )
}
