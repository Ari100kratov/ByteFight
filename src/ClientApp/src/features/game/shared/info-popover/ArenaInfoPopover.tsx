import type { ReactNode } from "react"
import { Popover, PopoverAnchor, PopoverContent } from "@/components/ui/popover"
import type { InfoPopoverPosition, InfoPopoverSide } from "../../types/InfoPopoverPosition"

interface ArenaInfoPopoverProps {
  open: boolean
  position?: InfoPopoverPosition
  lastPosition?: InfoPopoverPosition
  defaultSide?: InfoPopoverSide
  contentClassName: string
  children: ReactNode
  onClose: () => void
}

export function ArenaInfoPopover({
  open,
  position,
  lastPosition,
  defaultSide = "right",
  contentClassName,
  children,
  onClose,
}: ArenaInfoPopoverProps) {
  const anchorPosition = position ?? lastPosition
  const side = position?.side ?? anchorPosition?.side ?? defaultSide

  return (
    <Popover
      open={open}
      onOpenChange={(value) => {
        if (!value) {
          onClose()
        }
      }}
    >
      {anchorPosition && (
        <PopoverAnchor asChild>
          <div
            className="pointer-events-none absolute size-1"
            style={{
              left: anchorPosition.x,
              top: anchorPosition.y,
            }}
          />
        </PopoverAnchor>
      )}

      {open && (
        <PopoverContent
          side={side}
          align="start"
          sideOffset={36}
          collisionPadding={24}
          className={contentClassName}
        >
          {children}
        </PopoverContent>
      )}
    </Popover>
  )
}
