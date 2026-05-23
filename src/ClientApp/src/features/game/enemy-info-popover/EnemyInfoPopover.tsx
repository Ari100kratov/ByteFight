import { useArenaEnemiesStore } from "../state/data/arena-enemies.store"
import { useEnemiesStore } from "../state/data/enemies.data.store"
import { useEnemySelectionStore } from "../state/ui/enemy.selection.store"
import { UnitPreview } from "@/features/unit-preview/UnitPreview"
import { Skull } from "lucide-react"
import { ArenaInfoPopover } from "../shared/info-popover/ArenaInfoPopover"

export function EnemyInfoPopover() {
  const selectedArenaEnemyId = useEnemySelectionStore((s) => s.selectedArenaEnemyId)
  const position = useEnemySelectionStore((s) => s.position)
  const lastPosition = useEnemySelectionStore((s) => s.lastPosition)
  const clearSelection = useEnemySelectionStore((s) => s.clearSelection)

  const arenaEnemy = useArenaEnemiesStore((s) =>
    selectedArenaEnemyId ? s.arenaEnemies[selectedArenaEnemyId] : undefined,
  )

  const enemy = useEnemiesStore((s) => (arenaEnemy ? s.enemies[arenaEnemy.enemyId] : undefined))

  const open = Boolean(enemy && position)

  return (
    <ArenaInfoPopover
      open={open}
      position={position}
      lastPosition={lastPosition}
      contentClassName="w-[clamp(360px,42vw,560px)] max-w-[calc(100vw-2rem)]"
      onClose={clearSelection}
    >
      {enemy && (
        <div className="flex flex-col gap-4">
          <div>
            <div className="flex items-center gap-2 text-base leading-none font-semibold">
              <Skull className="text-muted-foreground" />
              {enemy.name}
            </div>

            {enemy.description && (
              <p className="text-muted-foreground mt-2 text-sm">{enemy.description}</p>
            )}
          </div>

          <UnitPreview
            stats={enemy.stats}
            actionAssets={enemy.actionAssets}
            abilities={enemy.abilities}
          />
        </div>
      )}
    </ArenaInfoPopover>
  )
}
