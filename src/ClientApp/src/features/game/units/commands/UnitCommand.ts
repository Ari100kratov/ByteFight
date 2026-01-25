import type { UnitController } from "../controller/UnitController";

export interface UnitCommand {
  execute(unit: UnitController): Promise<void>;
}
