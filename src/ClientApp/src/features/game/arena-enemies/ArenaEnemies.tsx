import { EnemyAnimatedSprite } from "../enemy-sprite/EnemyAnimatedSprite"
import type { GridLayout } from "../grid/gridUtils"
import { useArenaEnemies } from "./useArenaEnemies"

type Props = {
  arenaId: string
  layout: GridLayout
}

export function ArenaEnemies({ arenaId, layout }: Props) {
  const { data: arenaEnemies } = useArenaEnemies(arenaId)

  if (!arenaEnemies || arenaEnemies.length === 0) 
    return null

  return (
    <>
      {arenaEnemies.map((arenaEnemy) => {
        const cell = layout.cells[arenaEnemy.position.y][arenaEnemy.position.x]
        return (
          <EnemyAnimatedSprite
            key={arenaEnemy.id}
            enemyId={arenaEnemy.enemyId}
            x={cell.x}
            y={cell.y}
            width={cell.width}
            height={cell.height}
          />
        )
      })}
    </>
  )
}
