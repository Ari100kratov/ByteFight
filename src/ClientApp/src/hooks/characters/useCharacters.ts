import { useEffect, useState } from "react"
import { getCharactersByCurrentUser, type Character } from "@/api/characters"

export function useCharacters() {
  const [characters, setCharacters] = useState<Character[]>([])
  const [loading, setLoading] = useState(true)
  const [error, setError] = useState<string | null>(null)

  useEffect(() => {
    let active = true

    getCharactersByCurrentUser()
      .then((data) => {
        if (active) {
          setCharacters(data)
          setError(null)
        }
      })
      .catch((err) => {
        if (active) setError(err.message)
      })
      .finally(() => {
        if (active) setLoading(false)
      })

    return () => {
      active = false
    }
  }, [])

  return { characters, loading, error }
}
