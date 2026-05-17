import * as signalR from "@microsoft/signalr"
import { gameHubUrl } from "@/shared/config/api"
import { useGameRuntimeStore } from "../state/game.runtime.store"

class GameHubConnection {
  private connection: signalR.HubConnection | null = null
  private currentSessionId: string | null = null
  private connectingPromise: Promise<void> | null = null

  public async connect(sessionId: string) {
    // Уже подключены к этой сессии
    if (
      this.connection &&
      this.currentSessionId === sessionId &&
      this.connection.state === signalR.HubConnectionState.Connected
    ) {
      return
    }

    // Если идет подключение — дождаться его
    if (this.connectingPromise) {
      return this.connectingPromise
    }

    // Если подключены к другой сессии — отключиться
    if (this.connection && this.currentSessionId && this.currentSessionId !== sessionId) {
      await this.disconnect(this.currentSessionId)
    }

    const connection = new signalR.HubConnectionBuilder()
      .withUrl(gameHubUrl())
      .withAutomaticReconnect()
      .build()

    this.connection = connection
    this.currentSessionId = sessionId

    connection.on("Tick", (turnLog) => {
      useGameRuntimeStore.getState().enqueueTurn(turnLog)
    })

    connection.on("Finished", (session) => {
      useGameRuntimeStore.getState().setSession(session)
    })

    this.connectingPromise = (async () => {
      await connection.start()
      await connection.invoke("JoinGame", sessionId)
    })()

    try {
      await this.connectingPromise
    } finally {
      this.connectingPromise = null
    }
  }

  public async disconnect(sessionId: string) {
    if (!this.connection) return

    if (this.connection.state !== signalR.HubConnectionState.Disconnected) {
      try {
        await this.connection.invoke("LeaveGame", sessionId)
      } catch (err) {
        console.error("Failed to leave game hub:", err)
      }
      await this.connection.stop()
    }

    this.connection = null
    this.currentSessionId = null
  }
}

export const gameHub = new GameHubConnection()
