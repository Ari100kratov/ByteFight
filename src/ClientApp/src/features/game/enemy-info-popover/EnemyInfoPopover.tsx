import { Popover, PopoverAnchor, PopoverContent } from "@/components/ui/popover"
import { useArenaEnemiesStore } from "../state/data/arena-enemies.store"
import { useEnemiesStore } from "../state/data/enemies.data.store"
import { SpriteAnimationPlayer } from "@/features/character-class-selector/components/SpriteAnimationPlayer"
import { CharacterStats } from "@/features/character-class-selector/components/CharacterStats"
import { useEnemySelectionStore } from "../state/data/enemy.selection.store"

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
        className="w-[520px]"
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

            <div className="flex flex-col gap-6 md:flex-row">
              <div className="flex items-center justify-center p-4">
                <SpriteAnimationPlayer actionAssets={enemy.actionAssets} />
              </div>

              <CharacterStats stats={enemy.stats} />
            </div>
          </div>
        )}
      </PopoverContent>
    </Popover>
  )
}