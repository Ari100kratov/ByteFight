import { useEffect } from "react"
import { EnemyAnimatedSprite } from "../enemy-sprite/EnemyAnimatedSprite"
import { useEnemiesStore } from "../state/data/enemies.data.store"
import { useArenaEnemies } from "./useArenaEnemies"
import { fetchEnemy } from "./fetchEnemy"
import { useEnemyStateStore } from "../state/game/enemy.state.store"
import { StatType } from "@/shared/types/stat"

export function ArenaEnemies() {
  const { data: arenaEnemies } = useArenaEnemies()
  const setEnemies = useEnemiesStore(s => s.setEnemies)
  const init = useEnemyStateStore(s => s.init)

  useEffect(() => {
    if (!arenaEnemies) return

    const uniqueEnemyIds = [...new Set(arenaEnemies.map(e => e.enemyId))]
    if (uniqueEnemyIds.length === 0) return

    Promise.all(uniqueEnemyIds.map(fetchEnemy))
      .then((enemiesData) => {
        setEnemies(enemiesData)

        const initPayload = arenaEnemies.map((arenaEnemy) => {
          const enemyData = enemiesData.find(e => e.id === arenaEnemy.enemyId)
          const health = enemyData?.stats.find(s => s.statType === StatType.Health)?.value
          const mana = enemyData?.stats.find(s => s.statType === StatType.Mana)?.value

          return {
            arenaEnemyId: arenaEnemy.id,
            position: arenaEnemy.position,
            maxHp: health ?? 0,
            maxMp: mana,
          }
        })

        init(initPayload)
      })
      .catch(console.error)
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
