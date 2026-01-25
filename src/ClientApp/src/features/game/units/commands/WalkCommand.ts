import type { WalkLogEntry } from "../../types/TurnLog";
import type { UnitController } from "../controller/UnitController";
import type { UnitCommand } from "./UnitCommand";

export class WalkCommand implements UnitCommand {
  private entry: WalkLogEntry

  constructor(entry: WalkLogEntry) {
    this.entry = entry
  }

  async execute(unit: UnitController) {
    await unit.walkTo(this.entry);
  }
}