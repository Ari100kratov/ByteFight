import { isAttack, isDeath, isWalk, type GameActionLogEntry } from "../types/TurnLog";
import { unitRegistry } from "../units/controller/UnitRegistry";

export async function playRuntimeLog(entry: GameActionLogEntry) {
  if (isWalk(entry)) {
    const actor = unitRegistry.get(entry.actorId);
    await actor.walkTo(entry);
    return;
  }

  if (isAttack(entry)) {
    const actor = unitRegistry.get(entry.actorId);
    const target = unitRegistry.get(entry.targetId);
    await actor.attack(target, entry);
    return;
  }

  if (isDeath(entry)) {
    const actor = unitRegistry.get(entry.actorId);
    await actor.death();
    return;
  }
}
