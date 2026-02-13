import { Spinner } from "@/components/ui/spinner"
import { useDebouncedCallback } from "@/shared/hooks/useDebouncedCallback"
import { lazy, Suspense, useEffect, useRef, useState } from "react"
import type * as monaco from "monaco-editor"
import { bindUserScriptDiagnostics, setupUserScriptIntellisense } from "./userScriptIntellisense"

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
  const disposeDiagnosticsRef = useRef<null | (() => void)>(null)

  useEffect(() => {
    setLocalValue(value)
  }, [value])

  const debouncedChange = useDebouncedCallback((val: string) => {
    onChange?.(val)
  }, 300)


  useEffect(() => {
    return () => {
      disposeDiagnosticsRef.current?.()
    }
  }, [])

  const handleMount = (editor: monaco.editor.IStandaloneCodeEditor, monacoApi: typeof monaco) => {
    setupUserScriptIntellisense(monacoApi)
    disposeDiagnosticsRef.current?.()
    disposeDiagnosticsRef.current = bindUserScriptDiagnostics(editor, monacoApi)
  }
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
        onMount={handleMount}
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
