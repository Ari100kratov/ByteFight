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

export interface CreateCharacterRequest {
  name: string
}

export async function createCharacter(data: CreateCharacterRequest) {
  const res = await fetch(`${import.meta.env.VITE_API_URL}/characters`, {
    method: "POST",
    headers: {
      "Content-Type": "application/json",
      Authorization: `Bearer ${localStorage.getItem("token")}`,
    },
    body: JSON.stringify(data),
  })

  if (!res.ok) {
    let msg = "Не удалось создать персонажа"
    try {
      const json = await res.json()
      msg = json.message ?? json.detail ?? msg
    } catch {}
    throw new Error(msg)
  }

  return await res.json() // возвращает id созданного персонажа
}

