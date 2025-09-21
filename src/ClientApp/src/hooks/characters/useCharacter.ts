import { useEffect, useState } from "react"
import { getCharacter, type Character } from "@/api/characters/getCharacter"

export default function useCharacter(id: string | undefined) {
  const [character, setCharacter] = useState<Character | null>(null)
  const [loading, setLoading] = useState(false)
  const [error, setError] = useState<string | null>(null)

  useEffect(() => {
    if (!id) return
    setLoading(true)
    setError(null)

    getCharacter(id)
      .then((data) => setCharacter(data))
      .catch((err) => {
        setError(err instanceof Error ? err.message : "Неизвестная ошибка")
      })
      .finally(() => setLoading(false))
  }, [id])

  return { character, loading, error }
}
