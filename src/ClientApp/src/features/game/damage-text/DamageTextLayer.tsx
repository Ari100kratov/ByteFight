import { useCharacterStateStore } from "../state/game/character.state.store";
import { useDamageTextStore } from "../state/game/damage.text.store";
import { useEnemyStateStore } from "../state/game/enemy.state.store";
import { useGridStore } from "../state/game/grid.state.store";
import { DamageText } from "./DamageText";

export function DamageTextLayer() {
  const layout = useGridStore(s => s.layout);
  const items = useDamageTextStore(s => s.items);
  const remove = useDamageTextStore(s => s.remove);

  const characterRuntime = useCharacterStateStore(s => s.runtime);
  const arenaEnemies = useEnemyStateStore(s => s.arenaEnemies);

  if (!layout) return null;

  const getRuntime = (unitId: string) => {
    if (characterRuntime?.id === unitId) return characterRuntime;

    return arenaEnemies[unitId];
  };

  return (
    <pixiContainer zIndex={99999}>
      {items.map(item => {
        const runtime = getRuntime(item.unitId);
        if (!runtime) return null;

        const cell = layout.cells[runtime.position.y][runtime.position.x];

        const x =
          runtime.renderPosition?.x ??
          cell.x + cell.width / 2;

        const y =
          runtime.renderPosition?.y ??
          cell.y + cell.height / 2;

        return (
          <DamageText
            key={item.id}
            value={item.value}
            x={x}
            y={y - 20}
            onComplete={() => remove(item.id)}
          />
        );
      })}
    </pixiContainer>
  );
}