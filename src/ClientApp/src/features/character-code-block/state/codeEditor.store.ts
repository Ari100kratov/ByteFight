import { create } from "zustand"
import { ChangeStatus, type EditableCode } from "../types"

type CodeEditorState = {
  codes: EditableCode[]
  baselineCodes: EditableCode[]
  activeCodeId?: string

  replaceFromServer: (codes: EditableCode[]) => void

  setActiveCode: (id?: string) => void
  updateCodeSource: (id: string, sourceCode: string) => void
  renameCode: (id: string, name: string) => void
  createCode: (code: EditableCode) => void
  markCodeDeleted: (id: string) => void
  resetToBaseline: () => void

  getActiveCode: () => EditableCode | undefined
}

export const useCodeEditorStore = create<CodeEditorState>((set, get) => ({
  codes: [],
  baselineCodes: [],

  replaceFromServer: (codes) =>
    set(state => {
      const keepActive =
        state.activeCodeId && codes.some(c => c.id === state.activeCodeId)

      return {
        baselineCodes: codes,
        codes,
        activeCodeId: keepActive ? state.activeCodeId : codes[0]?.id,
      }
    }),

  setActiveCode: (id) => set({ activeCodeId: id }),

  updateCodeSource: (id, sourceCode) =>
    set(state => ({
      codes: state.codes.map(c =>
        c.id === id
          ? {
            ...c,
            sourceCode,
            status:
              c.status === ChangeStatus.Created
                ? c.status
                : ChangeStatus.Updated,
          }
          : c
      ),
    })),

  renameCode: (id, name) =>
    set(state => ({
      codes: state.codes.map(c =>
        c.id === id
          ? {
            ...c,
            name,
            status:
              c.status === ChangeStatus.Created
                ? c.status
                : ChangeStatus.Updated,
          }
          : c
      ),
    })),

  createCode: (code) =>
    set(state => ({
      codes: [...state.codes, code],
      activeCodeId: code.id,
    })),

  markCodeDeleted: (id) =>
    set(state => {
      const code = state.codes.find(c => c.id === id)
      if (!code) return state

      return {
        codes:
          code.status === ChangeStatus.Created
            ? state.codes.filter(c => c.id !== id)
            : state.codes.map(c =>
              c.id === id ? { ...c, status: ChangeStatus.Deleted } : c
            ),
        activeCodeId:
          state.activeCodeId === id
            ? state.codes.find(c => c.id !== id)?.id
            : state.activeCodeId,
      }
    }),

  resetToBaseline: () =>
    set(state => ({
      codes: state.baselineCodes,
      activeCodeId: state.baselineCodes.some(c => c.id === state.activeCodeId)
        ? state.activeCodeId
        : state.baselineCodes[0]?.id,
    })),

  getActiveCode: () => {
    const { codes, activeCodeId } = get()
    return codes.find(c => c.id === activeCodeId)
  },
}))

