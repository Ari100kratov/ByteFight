import { Target, Zap } from "lucide-react"
import {
  AbilityStatType,
  AbilityType,
  type AbilityDto,
} from "@/shared/types/ability"

interface Props {
  abilities: AbilityDto[]
}

export function CharacterBasicAttacks({ abilities }: Props) {
  const melee = abilities.find((x) => x.type === AbilityType.BasicMeleeAttack)
  const ranged = abilities.find((x) => x.type === AbilityType.BasicRangedAttack)

  if (!melee && !ranged) {
    return null
  }

  return (
    <ul className="space-y-2">
      {melee && (
        <BasicAttackItem ability={melee} label="Ближняя атака" />
      )}
      {ranged && (
        <BasicAttackItem ability={ranged} label="Дальняя атака" />
      )}
    </ul>
  )
}

function BasicAttackItem({
  ability,
  label,
}: {
  ability: AbilityDto
  label: string
}) {
  const damage = getAbilityStat(ability, AbilityStatType.Damage)
  const range = getAbilityStat(ability, AbilityStatType.Range)

  return (
    <li className="rounded-lg border p-3 text-sm">
      <div className="text-xs text-muted-foreground">
        {label}
      </div>

      {ability.name && (
        <div className="mt-1 font-medium">
          {ability.name}
        </div>
      )}

      {ability.description && (
        <p className="mt-2 text-xs text-muted-foreground">
          {ability.description}
        </p>
      )}

      <div className="mt-3 flex flex-wrap gap-x-4 gap-y-2">
        <div className="flex min-w-fit items-center gap-1">
          <span className="shrink-0">
            <Zap size={16} color="#374151" />
          </span>
          <span className="text-muted-foreground">Урон:</span>
          <span className="font-medium">{damage ?? "–"}</span>
        </div>

        <div className="flex min-w-fit items-center gap-1">
          <span className="shrink-0">
            <Target size={16} color="#10b981" />
          </span>
          <span className="text-muted-foreground">Дальность:</span>
          <span className="font-medium">{range ?? "–"}</span>
        </div>
      </div>
    </li>
  )
}

function getAbilityStat(ability: AbilityDto, statType: AbilityStatType) {
  return ability.stats.find((x) => x.statType === statType)?.value
}