import { HeartPulse, Target, Zap } from "lucide-react"
import { AbilityStatType, AbilityType, type AbilityDto } from "@/shared/types/ability"

interface Props {
  abilities: AbilityDto[]
}

const basicAttackTypes: AbilityType[] = [
  AbilityType.BasicMeleeAttack,
  AbilityType.BasicRangedAttack,
]

export function CharacterAbilities({ abilities }: Props) {
  const nonBasicAbilities = abilities.filter((x) => !basicAttackTypes.includes(x.type))

  if (nonBasicAbilities.length === 0) {
    return null
  }

  return (
    <div className="space-y-2">
      <div className="text-muted-foreground text-xs font-medium">Способности</div>

      <ul className="space-y-2">
        {nonBasicAbilities.map((ability) => (
          <AbilityItem key={ability.name} ability={ability} />
        ))}
      </ul>
    </div>
  )
}

function AbilityItem({ ability }: { ability: AbilityDto }) {
  const damage = getAbilityStat(ability, AbilityStatType.Damage)
  const healing = getAbilityStat(ability, AbilityStatType.Healing)
  const range = getAbilityStat(ability, AbilityStatType.Range)

  return (
    <li className="rounded-lg border p-3 text-sm">
      {ability.name && <div className="font-medium">{ability.name}</div>}

      {ability.description && (
        <p className="text-muted-foreground mt-2 text-xs">{ability.description}</p>
      )}

      <div className="mt-3 flex flex-wrap gap-x-4 gap-y-2">
        {damage !== undefined && (
          <AbilityStat icon={<Zap size={16} color="#374151" />} label="Урон" value={damage} />
        )}

        {healing !== undefined && (
          <AbilityStat
            icon={<HeartPulse size={16} color="#ef4444" />}
            label="Лечение"
            value={healing}
          />
        )}

        {range !== undefined && (
          <AbilityStat
            icon={<Target size={16} color="#10b981" />}
            label="Дальность"
            value={range}
          />
        )}
      </div>
    </li>
  )
}

function AbilityStat({
  icon,
  label,
  value,
}: {
  icon: React.ReactNode
  label: string
  value: number
}) {
  return (
    <div className="flex min-w-fit items-center gap-1">
      <span className="shrink-0">{icon}</span>
      <span className="text-muted-foreground">{label}:</span>
      <span className="font-medium">{value}</span>
    </div>
  )
}

function getAbilityStat(ability: AbilityDto, statType: AbilityStatType) {
  return ability.stats.find((x) => x.statType === statType)?.value
}
