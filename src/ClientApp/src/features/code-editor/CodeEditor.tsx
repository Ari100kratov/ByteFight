import { Spinner } from "@/components/ui/spinner"
import { useDebouncedCallback } from "@/shared/hooks/useDebouncedCallback"
import "@/monaco-setup"
import { lazy, Suspense, useEffect, useRef, useState } from "react"
import type * as monaco from "monaco-editor"
import { bindUserScriptDiagnostics, setupUserScriptIntellisense } from "./userScriptIntellisense"

const MonacoEditor = lazy(() => import("@monaco-editor/react"))

interface Props {
  value: string
  onChange?: (value: string) => void
}

interface EditorDraft {
  sourceValue: string
  value: string
}

export function CodeEditor({ value, onChange }: Props) {
  const [draft, setDraft] = useState<EditorDraft | null>(null)
  const disposeDiagnosticsRef = useRef<null | (() => void)>(null)
  const localValue = draft?.sourceValue === value ? draft.value : value

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
    setDraft({
      sourceValue: value,
      value: newValue,
    })
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
          smoothScrolling: true,

          fixedOverflowWidgets: true,

          quickSuggestions: {
            other: true,
            comments: false,
            strings: false,
          },

          parameterHints: {
            enabled: true,
          },

          suggestOnTriggerCharacters: true,
          acceptSuggestionOnEnter: "on",
        }}
      />
    </Suspense>
  )
}

function EditorLoader() {
  return (
    <div className="text-muted-foreground bg-muted/40 flex flex-1 items-center justify-center rounded-md text-sm">
      <Spinner /> Загрузка редактора...
    </div>
  )
}
