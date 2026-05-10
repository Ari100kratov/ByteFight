import { FlaskConical, HeartPulse, Sparkles } from "lucide-react"
import { Popover, PopoverAnchor, PopoverContent } from "@/components/ui/popover"
import { useArenaItemsStateStore } from "../state/game/arena-items.state.store"
import { useArenaItemSelectionStore } from "../state/ui/arena-item.selection.store"

export function ArenaItemInfoPopover() {
  const selectedPlacedItemId = useArenaItemSelectionStore(s => s.selectedPlacedItemId)
  const position = useArenaItemSelectionStore(s => s.position)
  const clearSelection = useArenaItemSelectionStore(s => s.clearSelection)

  const item = useArenaItemsStateStore(s =>
    selectedPlacedItemId
      ? s.items.find(x => x.placedItemId === selectedPlacedItemId)
      : undefined
  )

  const open = Boolean(item && position)

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
        className="w-[clamp(280px,32vw,420px)] max-w-[calc(100vw-2rem)]"
      >
        {item && (
          <div className="flex flex-col gap-4">
            <div>
              <div className="flex items-center gap-2 text-base font-semibold leading-none">
                <FlaskConical className="text-muted-foreground" />
                {item.name}
              </div>

              {item.description && (
                <p className="mt-2 text-sm text-muted-foreground">
                  {item.description}
                </p>
              )}
            </div>

            <div className="rounded-lg border bg-muted/30 p-3">
              <div className="mb-2 flex items-center gap-2 text-xs font-semibold uppercase text-muted-foreground">
                <Sparkles className="size-4" />
                Эффект
              </div>

              <div className="flex items-center justify-between gap-3 text-sm">
                <div className="flex items-center gap-2">
                  <HeartPulse className="size-4 text-red-500" />
                  <span>Восстановление здоровья</span>
                </div>

                <span className="font-semibold">
                  +{item.value}
                </span>
              </div>
            </div>
          </div>
        )}
      </PopoverContent>
    </Popover>
  )
}