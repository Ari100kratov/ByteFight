import type { AbilityDto } from "@/shared/types/ability"
import type { ActionAssetDto } from "@/shared/types/action"
import type { StatDto } from "@/shared/types/stat"
import { SpriteAnimationPlayer } from "./components/SpriteAnimationPlayer"
import { CharacterStats } from "./components/CharacterStats"
import { CharacterBasicAttacks } from "./components/CharacterBasicAttacks"
import { CharacterAbilities } from "./components/CharacterAbilities"

type Props = {
  stats: StatDto[]
  actionAssets: ActionAssetDto[]
  abilities: AbilityDto[]
}

export function UnitPreview({ stats, actionAssets, abilities }: Props) {
  return (
    <div className="flex flex-col gap-2 md:flex-row">
      <div className="flex items-center justify-center p-4">
        <SpriteAnimationPlayer
          actionAssets={actionAssets}
          abilities={abilities}
        />
      </div>

      <div className="flex flex-1 flex-col gap-2 justify-start">
        <CharacterStats stats={stats} />
        <CharacterBasicAttacks abilities={abilities} />
        <CharacterAbilities abilities={abilities} />
      </div>
    </div>
  )
}