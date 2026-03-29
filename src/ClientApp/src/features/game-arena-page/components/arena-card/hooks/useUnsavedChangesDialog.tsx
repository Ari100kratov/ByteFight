import { useCallback, useState } from "react"
import { useCodeEditorStore } from "@/features/character-code-block/state/codeEditor.store"
import {
  AlertDialog,
  AlertDialogAction,
  AlertDialogCancel,
  AlertDialogContent,
  AlertDialogDescription,
  AlertDialogFooter,
  AlertDialogHeader,
  AlertDialogTitle,
} from "@/components/ui/alert-dialog"
import { ChangeStatus } from "@/features/character-code-block/types"

type UseUnsavedChangesConfirmOptions = {
  onConfirm: () => void
  title?: string
  description?: string
  confirmText?: string
  cancelText?: string
}

export function useUnsavedChangesConfirm({
  onConfirm,
  title = "Есть несохраненные изменения",
  description = "В пользовательском коде есть несохраненные изменения. При продолжении они могут быть потеряны.",
  confirmText = "Продолжить без сохранения",
  cancelText = "Остаться",
}: UseUnsavedChangesConfirmOptions) {
  const codes = useCodeEditorStore(s => s.codes)
  const hasUnsavedChanges = codes.some(
    c => c.status !== ChangeStatus.Unchanged
  )

  const [open, setOpen] = useState(false)

  const confirm = useCallback(() => {
    if (hasUnsavedChanges) {
      setOpen(true)
      return
    }

    onConfirm()
  }, [hasUnsavedChanges, onConfirm])

  const dialog = (
    <AlertDialog open={open} onOpenChange={setOpen}>
      <AlertDialogContent>
        <AlertDialogHeader>
          <AlertDialogTitle>{title}</AlertDialogTitle>
          <AlertDialogDescription>{description}</AlertDialogDescription>
        </AlertDialogHeader>

        <AlertDialogFooter>
          <AlertDialogCancel>{cancelText}</AlertDialogCancel>
          <AlertDialogAction onClick={onConfirm}>
            {confirmText}
          </AlertDialogAction>
        </AlertDialogFooter>
      </AlertDialogContent>
    </AlertDialog>
  )

  return {
    confirm,
    dialog,
    hasUnsavedChanges,
  }
}