import { create } from "zustand"
import type { ActionType } from "@/shared/types/action"
import type { CharacterResponse } from "@/features/game-arena-page/hooks/useCharacterDetails"

export type Character = CharacterResponse

type CharacterState = {
  character?: Character
  setCharacter: (character: CharacterResponse) => void
  getSpriteAnimation: (actionType?: ActionType) => Character["class"]["actionAssets"][number]["spriteAnimation"] | undefined
}

export const useCharacterStore = create<CharacterState>((set, get) => ({
  character: undefined,
  setCharacter: (characterResponse) => {
    const character: Character = {
      ...characterResponse,
    }
    set({ character: character })
  },
  getSpriteAnimation: (actionType) => {
    if (!actionType) return undefined
    const character = get().character
    if (!character) return undefined
    return character.class.actionAssets.find(a => a.actionType === actionType && a.variant === 0)?.spriteAnimation
  },
}))
