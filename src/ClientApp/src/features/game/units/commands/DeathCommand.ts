import type { UnitController } from "../controller/UnitController";
import type { UnitCommand } from "./UnitCommand";

export class DeathCommand implements UnitCommand {
  async execute(unit: UnitController) {
    await unit.death();
  }
}