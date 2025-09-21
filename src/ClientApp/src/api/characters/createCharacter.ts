export interface CreateCharacterRequest {
  name: string
}

export default async function createCharacter(data: CreateCharacterRequest) {
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