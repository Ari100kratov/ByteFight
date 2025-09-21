export type Character = {
  id: string
  name: string
  userId: string
}

export async function getCharactersByCurrentUser(): Promise<Character[]> {
  const res = await fetch(
    `${import.meta.env.VITE_API_URL}/characters/by-current-user`,
    {
      method: "GET",
      headers: {
        "Content-Type": "application/json",
        Authorization: `Bearer ${localStorage.getItem("token")}`,
      },
    }
  )

  if (!res.ok) {
    throw new Error("Не удалось загрузить персонажей")
  }

  return res.json()
}



