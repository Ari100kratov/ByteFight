import { useEffect, useCallback } from "react"
import type { UseQueryResult } from "@tanstack/react-query"
import type { ApiException } from "@/shared/lib/apiFetch"
import { ChangeStatus } from "../types"
import { useCodeEditorStore } from "../state/codeEditor.store"
import type { CharacterCodeResponse } from "./useCharacterCodes"
import type { CodeTemplateResponse } from "./useCodeTemplate"

export function useCodeEditor(
  codesQuery: UseQueryResult<CharacterCodeResponse[], ApiException>,
  templateQuery: UseQueryResult<CodeTemplateResponse, ApiException>,
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
      codesQuery.data.map((code) => ({
        ...code,
        sourceCode: code.sourceCode ?? "",
        status: ChangeStatus.Unchanged,
      })),
    )
  }, [codesQuery.data, replaceFromServer])

  const addCode = useCallback(async () => {
    const { data } = await templateQuery.refetch()
    if (!data) return

    createCode({
      id: data.id,
      name: data.name,
      sourceCode: data.sourceCode,
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
