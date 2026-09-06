import * as signalR from '@microsoft/signalr'

import { gameHubUrl } from '@/shared/config/api'

import { useBattleStore } from '../state/battle.store'
import type { BattleStateDto, TurnLogDto } from '../types'
import type { HexPosition } from '../hex'

/**
 * Подключение к боевому хабу: получает состояние и журнал,
 * отправляет команды игрока в ручном режиме.
 */
class BattleHubConnection {
  private connection: signalR.HubConnection | null = null
  private currentSessionId: string | null = null
  private connectingPromise: Promise<void> | null = null

  public async connect(sessionId: string): Promise<void> {
    if (
      this.connection &&
      this.currentSessionId === sessionId &&
      this.connection.state === signalR.HubConnectionState.Connected
    ) {
      return
    }

    if (this.connectingPromise) {
      return this.connectingPromise
    }

    if (this.connection && this.currentSessionId && this.currentSessionId !== sessionId) {
      await this.disconnect(this.currentSessionId)
    }

    const connection = new signalR.HubConnectionBuilder()
      .withUrl(gameHubUrl())
      .withAutomaticReconnect()
      .build()

    this.connection = connection
    this.currentSessionId = sessionId

    connection.on('State', (state: BattleStateDto) => {
      useBattleStore.getState().applyState(state)
    })

    connection.on('Tick', (turnLog: TurnLogDto) => {
      useBattleStore.getState().appendLogs(turnLog.logs)
    })

    connection.on('Finished', () => {
      useBattleStore.getState().setFinished()
    })

    this.connectingPromise = (async () => {
      await connection.start()
      await connection.invoke('JoinGame', sessionId)
    })()

    try {
      await this.connectingPromise
    } finally {
      this.connectingPromise = null
    }
  }

  public async disconnect(sessionId: string): Promise<void> {
    if (!this.connection) return

    if (this.connection.state !== signalR.HubConnectionState.Disconnected) {
      try {
        await this.connection.invoke('LeaveGame', sessionId)
      } catch (err: unknown) {
        console.error('Не удалось покинуть боевой хаб:', err)
      }
      await this.connection.stop()
    }


    this.connection = null
    this.currentSessionId = null
  }

  public async submitMove(sessionId: string, path: HexPosition[]): Promise<void> {
    await this.invoke(sessionId, 'SubmitMove', sessionId, path.map((h) => ({ x: h.x, y: h.y })))
  }

  public async submitAbility(sessionId: string, abilityType: number, target: HexPosition): Promise<void> {
    await this.invoke(sessionId, 'SubmitAbility', sessionId, abilityType, target.x, target.y)
  }

  public async submitEndTurn(sessionId: string): Promise<void> {
    await this.invoke(sessionId, 'SubmitEndTurn', sessionId)
  }

  private async invoke(_sessionId: string, method: string, ...args: unknown[]): Promise<void> {
    if (this.connection?.state !== signalR.HubConnectionState.Connected) {
      console.warn(`Боевой хаб не подключен, команда ${method} отброшена`)

      return
    }

    try {
      await this.connection.invoke(method, ...args)
    } catch (err: unknown) {
      console.error(`Команда ${method} отклонена сервером:`, err)
    }
  }
}

export const battleHub = new BattleHubConnection()
