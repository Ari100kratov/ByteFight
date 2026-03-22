import { useEffect, useState } from "react"

type CharacterListItem = {
  id: string
}

const SELECTED_CHARACTER_STORAGE_KEY = "selected-character-id"

type Params<TCharacter extends CharacterListItem> = {
  characters?: TCharacter[]
  sessionCharacterId?: string
}

export function useSelectedCharacterId<TCharacter extends CharacterListItem>({
  characters,
  sessionCharacterId,
}: Params<TCharacter>) {
  const [selectedCharacterId, setSelectedCharacterId] = useState<string | undefined>(() => {
    const savedCharacterId = localStorage.getItem(SELECTED_CHARACTER_STORAGE_KEY)
    return savedCharacterId ?? undefined
  })

  // Персонаж из активной/завершенной сессии приоритетнее локального выбора
  useEffect(() => {
    if (!sessionCharacterId) return

    setSelectedCharacterId(prev =>
      prev === sessionCharacterId ? prev : sessionCharacterId
    )
  }, [sessionCharacterId])

  // Если список персонажей загружен и текущий выбор отсутствует в нем — очищаем
  useEffect(() => {
    if (!characters) return
    if (!selectedCharacterId) return

    const exists = characters.some(character => character.id === selectedCharacterId)
    if (!exists) {
      setSelectedCharacterId(undefined)
      localStorage.removeItem(SELECTED_CHARACTER_STORAGE_KEY)
    }
  }, [characters, selectedCharacterId])

  // Если персонаж не выбран вообще — автоматически берем первого из списка
  useEffect(() => {
    if (!characters?.length) return
    if (selectedCharacterId) return
    if (sessionCharacterId) return

    setSelectedCharacterId(characters[0].id)
  }, [characters, selectedCharacterId, sessionCharacterId])

  // Сохраняем актуальный выбор
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