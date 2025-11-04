import { Spinner } from "@/components/ui/spinner"
import { useDebouncedCallback } from "@/shared/hooks/useDebouncedCallback"
import { lazy, Suspense, useEffect, useState } from "react"

const MonacoEditor = lazy(() => import("@monaco-editor/react"))

type Props = {
  value: string
  onChange?: (value: string) => void
}

export function CodeEditor({
  value,
  onChange,
}: Props) {

  const [localValue, setLocalValue] = useState(value)

  useEffect(() => {
    setLocalValue(value)
  }, [value])

  const debouncedChange = useDebouncedCallback((val: string) => {
    onChange?.(val)
  }, 300)

  const handleChange = (val: string | undefined) => {
    const newValue = val ?? ""
    setLocalValue(newValue)
    debouncedChange(newValue)
  }

  return (
    <Suspense fallback={<EditorLoader />}>
      <MonacoEditor
        language="csharp"
        theme="light"
        loading={<EditorLoader />}
        value={localValue}
        onChange={handleChange}
        options={{
          minimap: { enabled: false },
          scrollBeyondLastLine: false,
          fontSize: 14,
          automaticLayout: true,
          smoothScrolling: true
        }}
      />
    </Suspense>
  )
}

function EditorLoader() {
  return (
    <div className="flex flex-1 items-center justify-center text-sm text-muted-foreground bg-muted/40 rounded-md">
      <Spinner /> Загрузка редактора...
    </div>
  )
}
