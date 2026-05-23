import { FlaskConical, HeartPulse, Sparkles } from "lucide-react"
import { useArenaItemsStateStore } from "../state/game/arena-items.state.store"
import { useArenaItemSelectionStore } from "../state/ui/arena-item.selection.store"
import { ArenaInfoPopover } from "../shared/info-popover/ArenaInfoPopover"

export function ArenaItemInfoPopover() {
  const selectedPlacedItemId = useArenaItemSelectionStore((s) => s.selectedPlacedItemId)
  const position = useArenaItemSelectionStore((s) => s.position)
  const lastPosition = useArenaItemSelectionStore((s) => s.lastPosition)
  const clearSelection = useArenaItemSelectionStore((s) => s.clearSelection)

  const item = useArenaItemsStateStore((s) =>
    selectedPlacedItemId ? s.items.find((x) => x.placedItemId === selectedPlacedItemId) : undefined,
  )

  const open = Boolean(item && position)

  return (
    <ArenaInfoPopover
      open={open}
      position={position}
      lastPosition={lastPosition}
      contentClassName="w-[clamp(280px,32vw,420px)] max-w-[calc(100vw-2rem)]"
      onClose={clearSelection}
    >
      {item && (
        <div className="flex flex-col gap-4">
          <div>
            <div className="flex items-center gap-2 text-base leading-none font-semibold">
              <FlaskConical className="text-muted-foreground" />
              {item.name}
            </div>

            {item.description && (
              <p className="text-muted-foreground mt-2 text-sm">{item.description}</p>
            )}
          </div>

          <div className="bg-muted/30 rounded-lg border p-3">
            <div className="text-muted-foreground mb-2 flex items-center gap-2 text-xs font-semibold uppercase">
              <Sparkles className="size-4" />
              Эффект
            </div>

            <div className="flex items-center justify-between gap-3 text-sm">
              <div className="flex items-center gap-2">
                <HeartPulse className="size-4 text-red-500" />
                <span>Восстановление здоровья</span>
              </div>

              <span className="font-semibold">+{item.value}</span>
            </div>
          </div>
        </div>
      )}
    </ArenaInfoPopover>
  )
}
