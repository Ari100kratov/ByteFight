import { Bot } from "lucide-react"
import { UnitPreview } from "@/features/unit-preview/UnitPreview"
import { ArenaInfoPopover } from "../shared/info-popover/ArenaInfoPopover"
import { useCharacterStore } from "../state/data/character.data.store"
import { useCharacterSelectionStore } from "../state/ui/character.selection.store"

export function CharacterInfoPopover() {
  const selectedCharacterId = useCharacterSelectionStore((s) => s.selectedCharacterId)
  const position = useCharacterSelectionStore((s) => s.position)
  const lastPosition = useCharacterSelectionStore((s) => s.lastPosition)
  const clearSelection = useCharacterSelectionStore((s) => s.clearSelection)
  const character = useCharacterStore((s) => s.character)

  const open = Boolean(character && selectedCharacterId === character.id && position)
  const specMeta = character
    ? [character.spec.className, character.spec.name].filter(Boolean).join(" · ")
    : ""

  return (
    <ArenaInfoPopover
      open={open}
      position={position}
      lastPosition={lastPosition}
      contentClassName="w-[clamp(360px,42vw,560px)] max-w-[calc(100vw-2rem)]"
      onClose={clearSelection}
    >
      {character && (
        <div className="flex flex-col gap-4">
          <div>
            <div className="flex items-center gap-2 text-base leading-none font-semibold">
              <Bot className="text-muted-foreground" />
              {character.name}
            </div>

            {specMeta && <p className="text-muted-foreground mt-2 text-sm">{specMeta}</p>}

            {character.spec.description && (
              <p className="text-muted-foreground mt-2 text-sm">{character.spec.description}</p>
            )}
          </div>

          <UnitPreview
            stats={character.spec.stats}
            actionAssets={character.spec.actionAssets}
            abilities={character.spec.abilities}
          />
        </div>
      )}
    </ArenaInfoPopover>
  )
}
