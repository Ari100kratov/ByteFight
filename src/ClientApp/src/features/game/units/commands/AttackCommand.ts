import type { UnitCommand } from "./UnitCommand";
import type { AttackLogEntry } from "../../types/TurnLog";
import type { UnitController } from "../controller/UnitController";

export class AttackCommand implements UnitCommand {
  private entry: AttackLogEntry;
  private target: UnitController;

  constructor(entry: AttackLogEntry, target: UnitController) {
    this.entry = entry;
    this.target = target;
  }

  async execute(unit: UnitController) {
    await unit.attack(this.target, this.entry);
  }
}
