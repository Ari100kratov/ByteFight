import { Circle, Footprints, Heart } from "lucide-react"
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
    StatType.MoveRange,
  ]

  const iconMap: Record<StatType, JSX.Element> = {
    [StatType.Health]: <Heart size={16} color="#ef4444" />,
    [StatType.Mana]: <Circle size={16} color="#3b82f6" />,
    [StatType.MoveRange]: <Footprints size={16} color="#facc15" />,
  }

  return (
    <div className="space-y-2">
      <h4 className="text-sm font-semibold uppercase text-muted-foreground">
        Характеристики
      </h4>

      <ul className="space-y-1">
        {allStatTypes.map((type) => {
          const value = statsMap.get(type)

          if (type === StatType.Mana && value === undefined) {
            return null
          }

          return (
            <li
              key={type}
              className="flex items-center justify-between gap-2 border-b pb-1 text-sm"
            >
              <div className="flex items-center gap-1">
                <span className="shrink-0">{iconMap[type]}</span>
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