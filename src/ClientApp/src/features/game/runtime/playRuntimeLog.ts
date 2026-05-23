import { useArenaItemsStateStore } from "../state/game/arena-items.state.store"
import {
  FloatingCombatTextKind,
  useFloatingCombatTextStore,
} from "../state/ui/floating.combat.text.store"
import {
  CombatVisualEffectKind,
  useCombatEffectsStore,
} from "../state/ui/combat.effects.store"
import {
  isAbilityUsed,
  isDeath,
  isItemPickedUp,
  isWalk,
  type GameActionLogEntry,
} from "../types/TurnLog"
import { unitRegistry } from "../units/controller/UnitRegistry"

export async function playRuntimeLog(entry: GameActionLogEntry) {
  if (isWalk(entry)) {
    const actor = unitRegistry.get(entry.actorId)
    await actor.walkTo(entry)
    return
  }

  if (isItemPickedUp(entry)) {
    const actor = unitRegistry.get(entry.actorId)

    useArenaItemsStateStore.getState().remove(entry.placedItemId)
    useCombatEffectsStore
      .getState()
      .addCellEffect(entry.position, CombatVisualEffectKind.ItemPickup, entry.id)
    useCombatEffectsStore
      .getState()
      .addUnitEffect(entry.actorId, CombatVisualEffectKind.HealingPulse, entry.id)

    useFloatingCombatTextStore
      .getState()
      .add(entry.actorId, entry.value, FloatingCombatTextKind.Healing)

    actor.updateHp(entry.actorHp)
    return
  }

  if (isAbilityUsed(entry)) {
    const actor = unitRegistry.get(entry.actorId)
    const target = unitRegistry.get(entry.targetId)
    await actor.useAbility(target, entry)
    return
  }

  if (isDeath(entry)) {
    const actor = unitRegistry.get(entry.actorId)
    await actor.death()
  }
}
