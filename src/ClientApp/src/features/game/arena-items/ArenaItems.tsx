import { useArenaItemsStateStore } from "../state/game/arena-items.state.store"
import { ArenaItemSprite } from "./ArenaItemSprite"

export function ArenaItems() {
  const items = useArenaItemsStateStore(s => s.items)

  return (
    <>
      {items.map(item => (
        <ArenaItemSprite
          key={item.placedItemId}
          item={item}
        />
      ))}
    </>
  )
}