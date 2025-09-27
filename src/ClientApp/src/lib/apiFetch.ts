export async function apiFetch<T>(path: string, options: RequestInit = {}): Promise<T> {
  const baseUrl = import.meta.env.VITE_API_URL
  const token = localStorage.getItem("token")

  const res = await fetch(`${baseUrl}${path}`, {
    ...options,
    headers: {
      "Content-Type": "application/json",
      Authorization: token ? `Bearer ${token}` : "",
      ...options.headers,
    },
  })

  if (res.status === 401) {
    // токен невалиден → чистим хранилище и отправляем на логин
    localStorage.removeItem("token")
    window.location.href = "/login"
    throw new Error("Требуется авторизация")
  }

  if (!res.ok) {
    let msg = "Ошибка при запросе"
    try {
      const json = await res.json()
      msg = json.message ?? json.detail ?? msg
    } catch {}
    throw new Error(msg)
  }

  return res.json()
}
