import * as signalR from "@microsoft/signalr";
import { useGameRuntimeStore } from "../state/game.runtime.store";

class GameHubConnection {
  private connection: signalR.HubConnection | null = null;

  public async connect(sessionId: string) {
    if (this.connection) return;

    this.connection = new signalR.HubConnectionBuilder()
      .withUrl(`/game-runtime-hub`)
      .withAutomaticReconnect()
      .build();

    this.connection.on("Tick", (turnLog) => {
      console.log("Tick:", turnLog);
      useGameRuntimeStore.getState().enqueueTurn(turnLog);
    });

    this.connection.on("Finished", (session) => {
      console.log("Finished:", session);
      useGameRuntimeStore.getState().setSession(session);
    });

    await this.connection.start();
    await this.connection.invoke("JoinGame", sessionId);
  }

  public async disconnect(sessionId: string) {
    if (!this.connection) return;
    console.log("disconnect:", sessionId);
    await this.connection.invoke("LeaveGame", sessionId);
    await this.connection.stop();
    this.connection = null;
  }
}

export const gameHub = new GameHubConnection();
