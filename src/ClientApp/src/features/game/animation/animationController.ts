import { useCharacterStateStore } from "../state/game/character.state.store";
import { useEnemyStateStore } from "../state/game/enemy.state.store";
import { useGridStore } from "../state/game/grid.state.store";
import type { UnitRuntime } from "../types/UnitRuntime";
import { ActionType } from "@/shared/types/action";
import type { WalkLogEntry } from "../types/TurnLog";

/**
 * Скорость перемещения в пикселях в секунду.
 * TODO: пока непонятно как будет себя вести для разных анимаций и разных спрайтов
 */
const DEFAULT_SPEED_PX_PER_SEC = 50;

function getController(unitId: string) {
  const char = useCharacterStateStore.getState().runtime;
  if (char && char.id === unitId) {
    return {
      set: (partial: Partial<UnitRuntime>) => useCharacterStateStore.getState().set(partial),
      get: () => useCharacterStateStore.getState().runtime,
    };
  }

  const enemyStore = useEnemyStateStore.getState();
  const enemy = Object.values(enemyStore.arenaEnemies).find(e => e.id === unitId);
  if (enemy) {
    return {
      set: (partial: Partial<UnitRuntime>) => enemyStore.set(unitId, partial),
      get: () => enemyStore.get(unitId),
    };
  }

  return null;
}

/**
 * Плавное перемещение юнита от текущей позиции к целевой
 */
export async function moveUnitSmooth(logEntry: WalkLogEntry) {
  const controller = getController(logEntry.actorId);
  if (!controller) throw new Error("Unit not found: " + logEntry.actorId);

  const layout = useGridStore.getState().layout;
  if (!layout) throw new Error("Layout not available");

  const runtime = controller.get();
  if (!runtime) throw new Error("Runtime not available for " + logEntry.actorId);

  // --- вычисляем пиксельные координаты начала и конца ---
  const startCell = layout.cells[runtime.position.y][runtime.position.x];
  const startPx = {
    x: startCell.x + startCell.width / 2,
    y: startCell.y + startCell.height - 10,
  };

  const targetCell = layout.cells[logEntry.to.y][logEntry.to.x];
  const targetPx = {
    x: targetCell.x + targetCell.width / 2,
    y: targetCell.y + targetCell.height - 10,
  };

  const dx = targetPx.x - startPx.x;
  const dy = targetPx.y - startPx.y;
  const dist = Math.hypot(dx, dy);
  const duration = (dist / DEFAULT_SPEED_PX_PER_SEC) * 1000; // ms

  // --- устанавливаем действие Walk и направление ---
  controller.set({ action: ActionType.Walk, facing: logEntry.facingDirection });

  // --- плавное перемещение через renderPosition ---
  return new Promise<void>((resolve) => {
    const startTime = performance.now();

    // сразу выставляем стартовую позицию
    controller.set({ renderPosition: { x: startPx.x, y: startPx.y } });

    let rafId = 0;
    const step = (now: number) => {
      const t = Math.min(1, (now - startTime) / Math.max(1, duration));
      const curX = startPx.x + dx * t;
      const curY = startPx.y + dy * t;

      controller.set({ renderPosition: { x: curX, y: curY } });

      if (t >= 1) {
        // завершаем ход: обновляем логическую позицию, сбрасываем renderPosition и возвращаем Idle
        controller.set({ position: logEntry.to, renderPosition: undefined, action: ActionType.Idle });
        cancelAnimationFrame(rafId);
        resolve();
        return;
      }

      rafId = requestAnimationFrame(step);
    };

    rafId = requestAnimationFrame(step);
  });
}
