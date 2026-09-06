import { beforeEach, describe, expect, it } from 'vitest'

import { useBattleStore } from './battle.store'
import type { BattleStateDto } from '../types'

function makeState(activeUnitId: string | null): BattleStateDto {
  return {
    round: 2,
    activeUnitId,
    turnOrder: ['player', 'enemy'],
    arena: {
      width: 4,
      height: 4,
      blocked: [],
      terrain: [],
    },
    units: [
      {
        id: 'player',
        name: 'Горислав',
        isPlayerSide: true,
        isDead: false,
        position: { x: 0, y: 0 },
        facing: 1,
        health: 100,
        maxHealth: 120,
        mana: 40,
        maxMana: 60,
        movePoints: 3,
        actions: 2,
        initiative: 10,
        statuses: [],
        cooldowns: {},
        abilities: [],
      },
      {
        id: 'enemy',
        name: 'Упырь',
        isPlayerSide: false,
        isDead: false,
        position: { x: 2, y: 2 },
        facing: 5,
        health: 55,
        maxHealth: 80,
        mana: 0,
        maxMana: 0,
        movePoints: 0,
        actions: 0,
        initiative: 6,
        statuses: [{ type: 2, turns: 2, magnitude: 7 }],
        cooldowns: {},
        abilities: [],
      },
    ],
  }
}

describe('battle store', () => {
  beforeEach(() => {
    useBattleStore.getState().reset()
  })

  it('ход игрока определяется по активному юниту', () => {
    const store = useBattleStore.getState()

    store.applyState(makeState('player'))
    expect(useBattleStore.getState().isPlayerTurn()).toBe(true)

    store.applyState(makeState('enemy'))
    expect(useBattleStore.getState().isPlayerTurn()).toBe(false)
  })

  it('смена активного юнита сбрасывает прицеливание', () => {
    const store = useBattleStore.getState()

    store.applyState(makeState('player'))
    store.beginTargeting({ ability: {} as never, targetHexes: [] })
    store.applyState(makeState('enemy'))

    expect(useBattleStore.getState().targeting).toBeNull()
  })

  it('журнал хранит последние записи', () => {
    const store = useBattleStore.getState()
    store.applyState(makeState('player'))

    store.appendLogs([
      { id: '1', entryType: 'IdleLogEntryDto' } as never,
      { id: '2', entryType: 'DeathLogEntryDto' } as never,
    ])

    const logs = useBattleStore.getState().logs

    expect(logs).toHaveLength(2)
    expect(logs[0].id).toBe('2')
  })

  it('поиск юнита по идентификатору', () => {
    useBattleStore.getState().applyState(makeState('player'))

    expect(useBattleStore.getState().unitById('enemy')?.name).toBe('Упырь')
    expect(useBattleStore.getState().unitById('missing')).toBeNull()
  })
})
