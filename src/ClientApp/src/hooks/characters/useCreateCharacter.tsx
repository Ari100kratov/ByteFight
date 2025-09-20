import { useState } from "react"
import { createCharacter } from "@/api/characters"

export function useCreateCharacter() {
  const [loading, setLoading] = useState(false)
  const [error, setError] = useState<string | null>(null)

  async function create(data: { name: string }) {
    if (!data.name?.trim()) {
      const msg = "Имя персонажа обязательно"
      setError(msg)
      throw new Error(msg)
    }

    setLoading(true)
    setError(null)
    try {
      const result = await createCharacter(data)
      return result
    } catch (err) {
      setError(err instanceof Error ? err.message : "Неизвестная ошибка")
      throw err
    } finally {
      setLoading(false)
    }
  }

  return { create, loading, error }
}
