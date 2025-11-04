import { useState, useEffect } from "react"
import { type LocalCode, ChangeStatus } from "../types"
import type { UseQueryResult } from "@tanstack/react-query";

export function useLocalCodes(codesQuery: UseQueryResult<any>, templateQuery: UseQueryResult<any>) {
  const [localCodes, setLocalCodes] = useState<LocalCode[]>([])
  const [activeTab, setActiveTab] = useState<string | undefined>(undefined)

  // загружаем данные с сервера
  useEffect(() => {
    if (codesQuery.data == null)
      return

    const newLocalCodes: LocalCode[] = codesQuery.data.map((c: any) => ({
      ...c,
      sourceCode: c.sourceCode ?? "",
      status: ChangeStatus.Unchanged,
    }))

    setLocalCodes(newLocalCodes)

    setActiveTab(prev => {
      if (prev && newLocalCodes.some(c => c.id === prev))
        return prev

      return newLocalCodes[0]?.id
    })
  }, [codesQuery.data])

  const addCode = async () => {
    const { data } = await templateQuery.refetch()
    if (!data) return

    const newCode: LocalCode = {
      id: data.id,
      name: data.name,
      sourceCode: data.sourceCode ?? "",
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
    if (codesQuery.data == null)
      return

    const newLocalCodes: LocalCode[] = codesQuery.data.map((c: any) => ({
      ...c,
      sourceCode: c.sourceCode ?? "",
      status: ChangeStatus.Unchanged,
    }))

    setLocalCodes(newLocalCodes)

    setActiveTab(prev => {
      if (prev && newLocalCodes.some(c => c.id === prev))
        return prev

      return newLocalCodes[0]?.id
    })
  }

  return { localCodes, activeTab, setActiveTab, addCode, deleteCode, renameCode, changeSource, resetChanges }
}
