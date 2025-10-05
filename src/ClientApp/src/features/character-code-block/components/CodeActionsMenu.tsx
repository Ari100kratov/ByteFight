import { useState } from "react"
import { Popover, PopoverTrigger, PopoverContent } from "@/components/ui/popover"
import { Edit, Trash, EllipsisVertical, Check, X } from "lucide-react"
import { ConfirmDialog } from "@/components/common/ConfirmDialog"
import { Button } from "@/components/ui/button"
import { Input } from "@/components/ui/input"

type Props = {
  name: string
  onRename: (newName: string) => void
  onDelete: () => void
}

export function CodeActionsMenu({ name, onRename, onDelete }: Props) {
  const [isEditing, setIsEditing] = useState(false)
  const [newName, setNewName] = useState(name)
  const [error, setError] = useState<string | null>(null)

  const handleSave = () => {
    const trimmed = newName.replace(/\s/g, "")
    if (!trimmed) {
      setError("Имя обязательно")
      return
    }
    if (trimmed.length > 32) {
      setError("Имя не должно превышать 32 символа")
      return
    }
    onRename(newName.trim())
    setIsEditing(false)
  }

  const handleCancel = () => {
    setNewName(name)
    setError(null)
    setIsEditing(false)
  }

  return (
    <Popover onOpenChange={(open) => {
      if (!open) {
        setIsEditing(false)
        setNewName(name)
        setError(null)
      }
    }}>
      <PopoverTrigger asChild>
        <button className="p-1 rounded hover:bg-gray-200 dark:hover:bg-gray-700">
          <EllipsisVertical size={16} />
        </button>
      </PopoverTrigger>
      <PopoverContent className="w-48 p-2">
        {isEditing ? (
          <div className="flex flex-col gap-2">
            <Input
              value={newName}
              onChange={e => setNewName(e.target.value)}
              placeholder="Введите имя вкладки"
              autoFocus
            />
            {error && <span className="text-red-500 text-xs">{error}</span>}
            <div className="flex justify-end gap-1">
              <Button size="sm" variant="ghost" onClick={handleCancel}>
                <X size={14} />
              </Button>
              <Button size="sm" onClick={handleSave}>
                <Check size={14} />
              </Button>
            </div>
          </div>
        ) : (
          <>
            <button
              className="flex items-center gap-2 w-full px-2 py-1 text-sm hover:bg-gray-100 rounded"
              onClick={() => setIsEditing(true)}
            >
              <Edit size={14} /> Переименовать
            </button>

            <ConfirmDialog
              trigger={
                <button className="flex items-center gap-2 w-full px-2 py-1 text-sm text-red-500 hover:bg-gray-100 rounded">
                  <Trash size={14} /> Удалить
                </button>
              }
              title="Удалить код?"
              description="После сохранения действие нельзя будет отменить."
              onConfirm={onDelete}
            />
          </>
        )}
      </PopoverContent>
    </Popover>
  )
}
