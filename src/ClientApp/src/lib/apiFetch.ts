import { clearAuth, getAccessToken, getRefreshToken, saveAuthTokens } from "./auth"

export async function apiFetch<T>(path: string, options: RequestInit = {}): Promise<T> {
  const baseUrl = import.meta.env.VITE_API_URL
  let accessToken = getAccessToken()

  const res = await fetch(`${baseUrl}${path}`, {
    ...options,
    headers: {
      "Content-Type": "application/json",
      Authorization: accessToken ? `Bearer ${accessToken}` : "",
      ...options.headers,
    },
  })

  if (res.status === 401) {
    try {
      accessToken = await refreshAccessToken()
      // повторяем запрос с новым access-токеном
      const retry = await fetch(`${baseUrl}${path}`, {
        ...options,
        headers: {
          "Content-Type": "application/json",
          Authorization: `Bearer ${accessToken}`,
          ...options.headers,
        },
      })
      if (!retry.ok) throw new Error("Ошибка при запросе после обновления токена")
      return retry.json()
    } catch {
      clearAuth()
      window.location.href = "/login"
      throw new Error("Требуется авторизация")
    }
  }

  if (!res.ok) {
    let msg = "Ошибка при запросе"
    try {
      const json = await res.json()
      msg = json.message ?? json.detail ?? msg
    } catch { }
    throw new Error(msg)
  }

  // статус 204 No Content или пустое тело
  if (res.status === 204) 
    return {} as T

  return res.json()
}

async function refreshAccessToken(): Promise<string> {
  const refreshToken = getRefreshToken()
  if (!refreshToken) throw new Error("Нет refresh-токена")

  const res = await fetch(`${import.meta.env.VITE_API_URL}/users/refresh-token`, {
    method: "POST",
    headers: { "Content-Type": "application/json" },
    body: JSON.stringify({ refreshToken }),
  })

  if (!res.ok) {
    clearAuth()
    throw new Error("Не удалось обновить токен")
  }

  const json = await res.json()

  saveAuthTokens(json.accessToken, json.refreshToken)

  return json.accessToken
}
