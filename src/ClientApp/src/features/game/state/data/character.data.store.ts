import { create } from "zustand"
import type { ActionType } from "@/shared/types/action"
import type { AbilityType } from "@/shared/types/ability"
import type { CharacterResponse } from "@/features/game-arena-page/components/select-character-card/hooks/useCharacterDetails"

export type Character = CharacterResponse

type CharacterState = {
  character?: Character
  setCharacter: (character: CharacterResponse) => void
  getSpriteAnimation: (
    actionType?: ActionType,
  ) => Character["spec"]["actionAssets"][number]["spriteAnimation"] | undefined
  getAbilitySpriteAnimation: (
    abilityType?: AbilityType,
    actionType?: ActionType,
  ) => Character["spec"]["abilities"][number]["actionAssets"][number]["spriteAnimation"] | undefined
  reset: () => void
}

export const useCharacterStore = create<CharacterState>((set, get) => ({
  character: undefined,

  setCharacter: (characterResponse) => {
    set({ character: characterResponse })
  },

  getSpriteAnimation: (actionType) => {
    if (!actionType) return undefined

    const character = get().character
    if (!character) return undefined

    const variants = character.spec.actionAssets.filter((a) => a.actionType === actionType)

    if (variants.length === 0) return undefined

    const randomIndex = Math.floor(Math.random() * variants.length)
    return variants[randomIndex].spriteAnimation
  },

  getAbilitySpriteAnimation: (abilityType, actionType) => {
    if (!abilityType || !actionType) return undefined

    const character = get().character
    if (!character) return undefined

    const ability = character.spec.abilities.find((a) => a.type === abilityType)

    if (!ability) return undefined

    const variants = ability.actionAssets.filter((a) => a.actionType === actionType)

    if (variants.length === 0) return undefined

    const randomIndex = Math.floor(Math.random() * variants.length)
    return variants[randomIndex].spriteAnimation
  },

  reset: () => {
    set({ character: undefined })
  },
}))
