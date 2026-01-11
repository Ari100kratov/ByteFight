import { Tabs, TabsContent, TabsList, TabsTrigger } from "@/components/ui/tabs"
import { Button } from "@/components/ui/button"
import { Plus } from "lucide-react"
import { CodeActionsMenu } from "./CodeActionsMenu"
import type { EditableCode } from "../types"
import { CodeEditor } from "@/features/code-editor/CodeEditor"

type Props = {
  codes: EditableCode[]
  activeTab: string | undefined
  onTabChange: (id: string) => void
  onAdd: () => void
  onRename: (id: string, name: string) => void
  onDelete: (id: string) => void,
  onChangeSource: (id: string, value: string) => void
}

export function CodeTabs({ codes, activeTab, onTabChange, onAdd, onRename, onDelete, onChangeSource }: Props) {
  return (
    <Tabs value={activeTab} onValueChange={onTabChange} className="flex-1 flex flex-col">
      {/* Вкладки */}
      <div
        className="relative flex items-center overflow-x-auto scrollbar-hide"
        onWheel={e => {
          e.currentTarget.scrollLeft += e.deltaY
        }}
      >
        <TabsList className="flex flex-nowrap gap-1">
          {codes.map(c => (
            <div key={c.id} className="flex items-center bg-muted rounded">
              <TabsTrigger value={c.id} asChild>
                <span className="flex-1 cursor-pointer select-none px-2 py-1 whitespace-nowrap">{c.name}</span>
              </TabsTrigger>

              <CodeActionsMenu
                name={c.name}
                onRename={(newName) => onRename(c.id, newName)}
                onDelete={() => onDelete(c.id)}
              />
            </div>
          ))}
          <Button size="sm" variant="ghost" onClick={onAdd}>
            <Plus size={16} />
          </Button>
        </TabsList>
      </div>

      {/* Контент вкладок */}
      {codes.map(c => (
        <TabsContent key={c.id} value={c.id} className="flex-1 flex flex-col mt-2">
          <CodeEditor
            value={c.sourceCode}
            onChange={(v) => onChangeSource(c.id, v)}
          />
        </TabsContent>
      ))}
    </Tabs>
  )
}
