import { useEffect, useCallback } from "react"
import type { UseQueryResult } from "@tanstack/react-query"
import { ChangeStatus } from "../types"
import { useCodeEditorStore } from "../state/codeEditor.store"

export function useCodeEditor(
  codesQuery: UseQueryResult<any>,
  templateQuery: UseQueryResult<any>
) {
  const {
    codes,
    activeCodeId,
    setActiveCode,
    createCode,
    markCodeDeleted,
    renameCode,
    updateCodeSource,
    resetToBaseline,
    replaceFromServer,
  } = useCodeEditorStore()

  useEffect(() => {
    if (!codesQuery.data) return

    replaceFromServer(
      codesQuery.data.map((c: any) => ({
        ...c,
        sourceCode: c.sourceCode ?? "",
        status: ChangeStatus.Unchanged,
      }))
    )
  }, [codesQuery.data, replaceFromServer])

  const addCode = useCallback(async () => {
    const { data } = await templateQuery.refetch()
    if (!data) return

    createCode({
      id: data.id,
      name: data.name,
      sourceCode: data.sourceCode ?? "",
      status: ChangeStatus.Created,
    })
  }, [templateQuery, createCode])

  return {
    codes,
    activeCodeId,
    setActiveCode,
    addCode,
    markCodeDeleted,
    renameCode,
    updateCodeSource,
    resetToBaseline,
  }
}

