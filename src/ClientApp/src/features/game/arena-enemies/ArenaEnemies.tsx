import { useEffect } from "react"
import { EnemyAnimatedSprite } from "../enemy-sprite/EnemyAnimatedSprite"
import { useEnemiesStore } from "../state/data/enemies.data.store"
import { useArenaEnemies } from "./useArenaEnemies"
import { fetchEnemy } from "./fetchEnemy"
import { useEnemyStateStore } from "../state/game/enemy.state.store"

export function ArenaEnemies() {
  const { data: arenaEnemies } = useArenaEnemies()
  const setEnemies = useEnemiesStore(s => s.setEnemies)
  const init = useEnemyStateStore(s => s.init)

  useEffect(() => {
    if (!arenaEnemies) return

    const uniqueEnemyIds = [...new Set(arenaEnemies.map(e => e.enemyId))]
    if (uniqueEnemyIds.length === 0) return

    Promise.all(uniqueEnemyIds.map(fetchEnemy))
      .then(setEnemies)
      .catch(console.error)

    const initPayload = arenaEnemies.map(e => ({
      arenaEnemyId: e.id,
      position: e.position,
    }))
    init(initPayload)
  }, [arenaEnemies])

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
