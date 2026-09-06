import { create } from 'zustand'

import type { BattleAbilityDto, BattleLogEntry, BattleStateDto, BattleUnitDto } from '../types'
import { hexKey, type HexPosition } from '../hex'

/** Режим прицеливания способности. */
export interface TargetingState {
  ability: BattleAbilityDto
  targetHexes: string[]
}

interface BattleStore {
  /** Полный снимок состояния от сервера. */
  state: BattleStateDto | null
  /** Записи журнала (последние сверху). */
  logs: BattleLogEntry[]
  /** Выбранный юнит для инспектора. */
  selectedUnitId: string | null
  /** Юнит под курсором. */
  hoveredUnitId: string | null
  /** Прицеливание способности. */
  targeting: TargetingState | null
  /** Путь перемещения при наведении. */
  previewPath: HexPosition[] | null
  /** Завершённая сессия. */
  finished: boolean

  applyState: (state: BattleStateDto) => void
  appendLogs: (entries: BattleLogEntry[]) => void
  selectUnit: (unitId: string | null) => void
  hoverUnit: (unitId: string | null) => void
  beginTargeting: (targeting: TargetingState | null) => void
  setPreviewPath: (path: HexPosition[] | null) => void
  setFinished: () => void
  reset: () => void

  /** Юнит игрока. */
  playerUnit: () => BattleUnitDto | null
  unitById: (id: string | null | undefined) => BattleUnitDto | null
  /** Ходит ли сейчас игрок. */
  isPlayerTurn: () => boolean
}

const MAX_LOGS = 120

export const useBattleStore = create<BattleStore>((set, get) => ({
  state: null,
  logs: [],
  selectedUnitId: null,
  hoveredUnitId: null,
  targeting: null,
  previewPath: null,
  finished: false,

  applyState: (state) => {
    // Смена активного юнита сбрасывает прицеливание и предпросмотр.
    const previous = get().state
    const activeChanged = previous?.activeUnitId !== state.activeUnitId

    set({
      state,
      targeting: activeChanged ? null : get().targeting,
      previewPath: activeChanged ? null : get().previewPath,
    })
  },

  appendLogs: (entries) => {
    if (entries.length === 0) return

    set((s) => ({
      logs: [...entries.reverse(), ...s.logs].slice(0, MAX_LOGS),
    }))
  },

  selectUnit: (unitId) => { set({ selectedUnitId: unitId }); },
  hoverUnit: (unitId) => { set({ hoveredUnitId: unitId }); },
  beginTargeting: (targeting) => { set({ targeting, previewPath: null }); },
  setPreviewPath: (previewPath) => { set({ previewPath }); },
  setFinished: () => { set({ finished: true }); },

  reset: () =>
    { set({
      state: null,
      logs: [],
      selectedUnitId: null,
      hoveredUnitId: null,
      targeting: null,
      previewPath: null,
      finished: false,
    }); },

  playerUnit: () => get().state?.units.find((u) => u.isPlayerSide && !u.isDead) ?? null,

  unitById: (id) => (id ? (get().state?.units.find((u) => u.id === id) ?? null) : null),

  isPlayerTurn: () => {
    const { state } = get()

    return state?.activeUnitId != null && state.units.some((u) => u.isPlayerSide && u.id === state.activeUnitId)
  },
}))

/** Помогает собрать множество занятых гексов. */
export function occupiedHexes(state: BattleStateDto): Set<string> {
  const result = new Set<string>()

  for (const unit of state.units) {
    if (!unit.isDead) {
      result.add(hexKey(unit.position))
    }
  }

  return result
}
