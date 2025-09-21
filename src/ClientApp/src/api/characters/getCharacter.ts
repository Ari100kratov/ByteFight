export type Character = {
  id: string
  name: string
  userId: string
}

export async function getCharacter(id: string): Promise<Character> {
  const token = localStorage.getItem("token")
  const res = await fetch(`${import.meta.env.VITE_API_URL}/characters/${id}`, {
    headers: {
      "Content-Type": "application/json",
      Authorization: token ? `Bearer ${token}` : "",
    },
  })

  if (!res.ok) {
    let msg = "Ошибка загрузки персонажа"
    try {
      const json = await res.json()
      msg = json.message ?? json.detail ?? msg
    } catch {}
    throw new Error(msg)
  }

  return res.json()
}
