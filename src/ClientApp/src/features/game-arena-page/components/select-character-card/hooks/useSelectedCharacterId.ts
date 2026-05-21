import { useEffect, useState } from "react"

interface CharacterListItem {
  id: string
}

const SELECTED_CHARACTER_STORAGE_KEY = "selected-character-id"

interface Params<TCharacter extends CharacterListItem> {
  characters?: TCharacter[]
  sessionCharacterId?: string
}

export function useSelectedCharacterId<TCharacter extends CharacterListItem>({
  characters,
  sessionCharacterId,
}: Params<TCharacter>) {
  const [manualSelectedCharacterId, setSelectedCharacterId] = useState<string | undefined>(() => {
    const savedCharacterId = localStorage.getItem(SELECTED_CHARACTER_STORAGE_KEY)
    return savedCharacterId ?? undefined
  })

  const selectedCharacterId = (() => {
    if (sessionCharacterId) {
      return sessionCharacterId
    }

    if (!characters) {
      return manualSelectedCharacterId
    }

    if (
      manualSelectedCharacterId &&
      characters.some((character) => character.id === manualSelectedCharacterId)
    ) {
      return manualSelectedCharacterId
    }

    if (characters.length === 0) {
      return undefined
    }

    return characters[0].id
  })()

  useEffect(() => {
    if (!selectedCharacterId) {
      localStorage.removeItem(SELECTED_CHARACTER_STORAGE_KEY)
      return
    }

    localStorage.setItem(SELECTED_CHARACTER_STORAGE_KEY, selectedCharacterId)
  }, [selectedCharacterId])

  return {
    selectedCharacterId,
    setSelectedCharacterId,
  }
}
