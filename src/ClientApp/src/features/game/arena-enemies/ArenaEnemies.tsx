import { useEffect } from "react"
import { EnemyAnimatedSprite } from "../enemy-sprite/EnemyAnimatedSprite"
import { useEnemiesStore } from "../state/data/enemies.data.store"
import { useArenaEnemies } from "./useArenaEnemies"
import { fetchEnemy } from "./fetchEnemy"

export function ArenaEnemies() {
  const { data: arenaEnemies } = useArenaEnemies()
  const setEnemies = useEnemiesStore(s => s.setEnemies)

  useEffect(() => {
    if (!arenaEnemies) return

    const uniqueEnemyIds = [...new Set(arenaEnemies.map(e => e.enemyId))]
    if (uniqueEnemyIds.length === 0) return

    Promise.all(uniqueEnemyIds.map(fetchEnemy))
      .then(setEnemies)
      .catch(console.error)
  }, [arenaEnemies, setEnemies])

  if (!arenaEnemies)
    return null

  return (
    <>
      {arenaEnemies.map((arenaEnemy) => {
        return (
          <EnemyAnimatedSprite
            key={arenaEnemy.id}
            arenaEnemyId={arenaEnemy.id}
          />
        )
      })}
    </>
  )
}
