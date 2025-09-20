export async function login(email: string, password: string) {
  const res = await fetch(`${import.meta.env.VITE_API_URL}/users/login`, {
    method: "POST",
    headers: { "Content-Type": "application/json" },
    body: JSON.stringify({ email, password }),
  })

  if (!res.ok) {
    let msg = "Ошибка входа"
    try {
      const json = await res.json()
      msg = json.message ?? json.detail ?? msg
    } catch {}
    throw new Error(msg)
  }

  const data = await res.json()
  return data.token as string
}

export async function register(user: {
  email: string
  firstName: string
  lastName: string
  password: string
}) {
  const res = await fetch(`${import.meta.env.VITE_API_URL}/users/register`, {
    method: "POST",
    headers: { "Content-Type": "application/json" },
    body: JSON.stringify(user),
  })

  if (!res.ok) {
    let msg = "Ошибка регистрации"
    try {
      const json = await res.json()
      msg = json.message ?? json.detail ?? msg
    } catch {}
    throw new Error(msg)
  }
}

