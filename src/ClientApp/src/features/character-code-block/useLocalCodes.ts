import { useState, useEffect } from "react"
import { type LocalCode, ChangeStatus } from "./types"
import type { UseQueryResult } from "@tanstack/react-query";

export function useLocalCodes(codesQuery: UseQueryResult<any>, templateQuery: UseQueryResult<any>) {
  const [localCodes, setLocalCodes] = useState<LocalCode[]>([])
  const [activeTab, setActiveTab] = useState<string | undefined>(undefined)

  // загружаем данные с сервера
  useEffect(() => {
    if (codesQuery.data) {
      setLocalCodes(
        codesQuery.data.map((c: any) => ({
          ...c,
          sourceCode: c.sourceCode ?? "",
          status: ChangeStatus.Unchanged,
        }))
      )
      setActiveTab(codesQuery.data[0]?.id)
    }
  }, [codesQuery.data])

  const addCode = async () => {
    await templateQuery.refetch()
    if (!templateQuery.data) return

    const newCode: LocalCode = {
      id: templateQuery.data.id,
      name: templateQuery.data.name,
      sourceCode: templateQuery.data.sourceCode ?? "",
      status: ChangeStatus.Created,
    }

    setLocalCodes(prev => [...prev, newCode])
    setActiveTab(newCode.id)
  }

  const deleteCode = (id: string) => {
  setLocalCodes(prev => {
    const code = prev.find(c => c.id === id)
    if (!code) return prev

    if (code.status === ChangeStatus.Created) {
      return prev.filter(c => c.id !== id)
    }

    return prev.map(c => (c.id === id ? { ...c, status: ChangeStatus.Deleted } : c))
  })

  if (activeTab === id) {
    const remaining = localCodes.filter(c => c.id !== id && c.status !== ChangeStatus.Deleted)
    setActiveTab(remaining[0]?.id)
  }
}

  const renameCode = (id: string, name: string) => {
    setLocalCodes(prev =>
      prev.map(c => (c.id === id ? { ...c, name, status: c.status === ChangeStatus.Created ? c.status : ChangeStatus.Updated } : c))
    )
  }

  const changeSource = (id: string, value: string) => {
    setLocalCodes(prev =>
      prev.map(c => (c.id === id ? { ...c, sourceCode: value, status: c.status === ChangeStatus.Created ? c.status : ChangeStatus.Updated } : c))
    )
  }

  const resetChanges = () => {
  if (!codesQuery.data) return

  setLocalCodes(
    codesQuery.data.map((c: any) => ({
      ...c,
      sourceCode: c.sourceCode ?? "",
      status: ChangeStatus.Unchanged,
    }))
  )
  setActiveTab(codesQuery.data[0]?.id)
}

  return { localCodes, activeTab, setActiveTab, addCode, deleteCode, renameCode, changeSource, resetChanges }
}
