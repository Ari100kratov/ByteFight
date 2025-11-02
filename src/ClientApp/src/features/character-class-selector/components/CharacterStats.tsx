import { Heart, Circle, Axe, Target, Footprints } from "lucide-react"
import { getStatName, type StatDto, StatType } from "@/shared/types/stat"
import type { JSX } from "react"

interface Props {
  stats: StatDto[]
}

export function CharacterStats({ stats }: Props) {
  const statsMap = new Map(stats.map((s) => [s.statType, s.value]))
  const allStatTypes: StatType[] = [
    StatType.Health,
    StatType.Mana,
    StatType.Attack,
    StatType.AttackRange,
    StatType.MoveRange,
  ]

  const iconMap: Record<StatType, JSX.Element> = {
    [StatType.Health]: <Heart size={16} color="#ef4444" />,
    [StatType.Mana]: <Circle size={16} color="#3b82f6" />,
    [StatType.Attack]: <Axe size={16} color="#374151" />,
    [StatType.AttackRange]: <Target size={16} color="#10b981" />,
    [StatType.MoveRange]: <Footprints size={16} color="#facc15" />,
  }

  return (
    <div className="flex-1 space-y-2">
      <h4 className="font-semibold text-sm uppercase text-muted-foreground">
        Характеристики
      </h4>
      <ul className="space-y-1">
        {allStatTypes.map((type) => {
          const value = statsMap.get(type)
          return (
            <li
              key={type}
              className="flex justify-between items-center border-b pb-1 text-sm gap-2"
            >
              <div className="flex items-center gap-1">
                {iconMap[type]}
                <span>{getStatName(type)}</span>
              </div>
              <span className="font-medium">{value ?? "–"}</span>
            </li>
          )
        })}
      </ul>
    </div>
  )
}
