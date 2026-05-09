import { Popover, PopoverAnchor, PopoverContent } from "@/components/ui/popover"
import { useArenaEnemiesStore } from "../state/data/arena-enemies.store"
import { useEnemiesStore } from "../state/data/enemies.data.store"
import { useEnemySelectionStore } from "../state/ui/enemy.selection.store"
import { UnitPreview } from "@/features/unit-preview/UnitPreview"

export function EnemyInfoPopover() {
  const selectedArenaEnemyId = useEnemySelectionStore(s => s.selectedArenaEnemyId)
  const position = useEnemySelectionStore(s => s.position)
  const clearSelection = useEnemySelectionStore(s => s.clearSelection)

  const arenaEnemy = useArenaEnemiesStore(s =>
    selectedArenaEnemyId ? s.arenaEnemies[selectedArenaEnemyId] : undefined
  )

  const enemy = useEnemiesStore(s =>
    arenaEnemy ? s.enemies[arenaEnemy.enemyId] : undefined
  )

  const open = Boolean(enemy && position)

  return (
    <Popover open={open} onOpenChange={(value) => !value && clearSelection()}>
      {position && (
        <PopoverAnchor asChild>
          <div
            className="pointer-events-none absolute size-1"
            style={{
              left: position.x,
              top: position.y,
            }}
          />
        </PopoverAnchor>
      )}

      <PopoverContent
        side="right"
        align="start"
        sideOffset={12}
        className="w-[clamp(360px,42vw,560px)] max-w-[calc(100vw-2rem)]"
      >
        {enemy && (
          <div className="flex flex-col gap-4">
            <div>
              <div className="text-base font-semibold leading-none">
                {enemy.name}
              </div>

              {enemy.description && (
                <p className="mt-2 text-sm text-muted-foreground">
                  {enemy.description}
                </p>
              )}
            </div>

            <UnitPreview
              stats={enemy.stats}
              actionAssets={enemy.actionAssets}
              abilities={enemy.abilities}
            />
          </div>
        )}
      </PopoverContent>
    </Popover>
  )
}