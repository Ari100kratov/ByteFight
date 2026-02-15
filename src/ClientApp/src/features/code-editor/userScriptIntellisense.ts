import type * as monaco from "monaco-editor"
import { apiFetch } from "@/shared/lib/apiFetch"

type PositionPayload = {
  line: number
  column: number
}

type IntellisenseDiagnostic = {
  code: string
  message: string
  severity: string
  startLine: number
  startColumn: number
  endLine: number
  endColumn: number
}

type IntellisenseCompletion = {
  label: string
  detail?: string
  kind?: string
  documentation?: string
}

type IntellisenseHover = {
  signature: string
  documentation?: string
  startLine: number
  startColumn: number
  endLine: number
  endColumn: number
}

type IntellisenseSignatureHelp = {
  signature: string
  documentation?: string
  activeParameter: number
  parameters: string[]
}

let initialized = false

export function setupUserScriptIntellisense(monacoApi: typeof monaco) {
  if (initialized) {
    return
  }

  initialized = true

  monacoApi.languages.registerCompletionItemProvider("csharp", {
    triggerCharacters: [".", "(", " "],
    provideCompletionItems: async (model, position) => {
      const suggestions = await request<IntellisenseCompletion[]>("/intellisense/csharp/completions", {
        sourceCode: model.getValue(),
        position: toPosition(position),
      })

      const word = model.getWordUntilPosition(position)
      const range: monaco.IRange = {
        startLineNumber: position.lineNumber,
        endLineNumber: position.lineNumber,
        startColumn: word.startColumn,
        endColumn: word.endColumn,
      }

      return {
        suggestions: (suggestions ?? []).map(item => ({
          label: item.label,
          kind: mapCompletionKind(monacoApi, item.kind),
          detail: item.detail,
          documentation: item.documentation,
          insertText: item.label,
          range,
        })),
      }
    },
  })

  monacoApi.languages.registerHoverProvider("csharp", {
    provideHover: async (model, position) => {
      const hover = await request<IntellisenseHover>("/intellisense/csharp/hover", {
        sourceCode: model.getValue(),
        position: toPosition(position),
      })

      if (!hover) {
        return null
      }

      const docs = hover.documentation ? `\n\n${hover.documentation.replace(/\n/g, "  \n")}` : ""
      return {
        range: new monacoApi.Range(hover.startLine, hover.startColumn, hover.endLine, hover.endColumn),
        contents: [{ value: `\`\`\`csharp\n${hover.signature}\n\`\`\`${docs}` }],
      }
    },
  })

  monacoApi.languages.registerSignatureHelpProvider("csharp", {
    signatureHelpTriggerCharacters: ["(", ","],
    signatureHelpRetriggerCharacters: [","],
    provideSignatureHelp: async (model, position) => {
      const payload = await request<IntellisenseSignatureHelp>("/intellisense/csharp/signature-help", {
        sourceCode: model.getValue(),
        position: toPosition(position),
      })

      if (!payload) {
        return {
          value: {
            signatures: [],
            activeSignature: 0,
            activeParameter: 0,
          },
          dispose: () => { },
        }
      }

      return {
        value: {
          signatures: [
            {
              label: payload.signature ?? "",
              documentation: payload.documentation ?? "",
              parameters: (payload.parameters ?? []).map(p => ({ label: p })),
            },
          ],
          activeSignature: 0,
          activeParameter: payload.activeParameter ?? 0,
        },
        dispose: () => { },
      }
    },
  })
}

export function bindUserScriptDiagnostics(editor: monaco.editor.IStandaloneCodeEditor, monacoApi: typeof monaco) {
  const updateDiagnostics = async () => {
    const model = editor.getModel()
    if (!model) return

    const diagnostics = await request<IntellisenseDiagnostic[]>("/intellisense/csharp/diagnostics", {
      sourceCode: model.getValue(),
    })

    const markers: monaco.editor.IMarkerData[] = (diagnostics ?? []).map(item => ({
      message: `[${item.code}] ${item.message}`,
      severity:
        item.severity.toLowerCase() === "error"
          ? monacoApi.MarkerSeverity.Error
          : monacoApi.MarkerSeverity.Warning,
      startLineNumber: item.startLine,
      startColumn: item.startColumn,
      endLineNumber: item.endLine,
      endColumn: item.endColumn,
    }))

    monacoApi.editor.setModelMarkers(model, "user-script", markers)
  }

  let timer: ReturnType<typeof setTimeout> | null = null

  const schedule = () => {
    if (timer) {
      clearTimeout(timer)
    }

    timer = setTimeout(() => {
      void updateDiagnostics()
    }, 350)
  }

  const subscription = editor.onDidChangeModelContent(schedule)
  schedule()

  return () => {
    if (timer) clearTimeout(timer)
    subscription.dispose()
    const model = editor.getModel()
    if (model) {
      monacoApi.editor.setModelMarkers(model, "user-script", [])
    }
  }
}

function toPosition(position: monaco.Position): PositionPayload {
  return {
    line: position.lineNumber,
    column: position.column,
  }
}

function mapCompletionKind(monacoApi: typeof monaco, rawKind?: string) {
  const kind = (rawKind ?? "").toLowerCase()

  if (kind.includes("method")) return monacoApi.languages.CompletionItemKind.Method
  if (kind.includes("property")) return monacoApi.languages.CompletionItemKind.Property
  if (kind.includes("class")) return monacoApi.languages.CompletionItemKind.Class
  if (kind.includes("struct")) return monacoApi.languages.CompletionItemKind.Struct
  if (kind.includes("enum")) return monacoApi.languages.CompletionItemKind.Enum
  if (kind.includes("field")) return monacoApi.languages.CompletionItemKind.Field
  if (kind.includes("keyword")) return monacoApi.languages.CompletionItemKind.Keyword
  if (kind.includes("variable") || kind.includes("local")) return monacoApi.languages.CompletionItemKind.Variable

  return monacoApi.languages.CompletionItemKind.Text
}

async function request<T>(path: string, body: unknown): Promise<T | null> {
  try {
    return await apiFetch<T>(path, {
      method: "POST",
      body: JSON.stringify(body),
    })
  } catch {
    return null
  }
}
