import * as signalR from "@microsoft/signalr";
import { useGameRuntimeStore } from "../state/game.runtime.store";

class GameHubConnection {
  private connection: signalR.HubConnection | null = null;
  private currentSessionId: string | null = null;
  private connectingPromise: Promise<void> | null = null;

  public async connect(sessionId: string) {
    // Уже подключены к этой сессии
    if (
      this.connection &&
      this.currentSessionId === sessionId &&
      this.connection.state === signalR.HubConnectionState.Connected
    ) {
      return;
    }

    // Если идет подключение — дождаться его
    if (this.connectingPromise) {
      return this.connectingPromise;
    }

    // Если подключены к другой сессии — отключиться
    if (this.connection && this.currentSessionId !== sessionId) {
      await this.disconnect(this.currentSessionId!);
    }

    this.connection = new signalR.HubConnectionBuilder()
      .withUrl(`/game-runtime-hub`)
      .withAutomaticReconnect()
      .build();

    this.currentSessionId = sessionId;

    this.connection.on("Tick", (turnLog) => {
      useGameRuntimeStore.getState().enqueueTurn(turnLog);
    });

    this.connection.on("Finished", (session) => {
      useGameRuntimeStore.getState().setSession(session);
    });

    this.connectingPromise = (async () => {
      await this.connection!.start();
      await this.connection!.invoke("JoinGame", sessionId);
    })();

    try {
      await this.connectingPromise;
    } finally {
      this.connectingPromise = null;
    }
  }

  public async disconnect(sessionId: string) {
    if (!this.connection) return;

    if (this.connection.state !== signalR.HubConnectionState.Disconnected) {
      try {
        await this.connection.invoke("LeaveGame", sessionId);
      } catch { }
      await this.connection.stop();
    }

    this.connection = null;
    this.currentSessionId = null;
  }
}

export const gameHub = new GameHubConnection();