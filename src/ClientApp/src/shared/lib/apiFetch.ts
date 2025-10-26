import { clearAuth, getAccessToken, getRefreshToken, saveAuthTokens } from "./auth"

/**
 * Формат ошибки, возвращаемый API (RFC 7807 / CustomResults.Problem)
 */
export type ApiError = {
  type: string
  title: string
  status: number
  detail: string
  errors?: Record<string, string[]>
  traceId?: string
}

/**
 * Кастомный класс ошибки для API
 */
export class ApiException extends Error {
  public readonly status: number
  public readonly type: string
  public readonly errors?: Record<string, string[]>
  public readonly traceId?: string

  constructor(problem: ApiError) {
    super(problem.detail || problem.title)
    this.name = "ApiException"
    this.status = problem.status
    this.type = problem.type
    this.errors = problem.errors
    this.traceId = problem.traceId
  }
}

/**
 * Универсальная функция для запросов к API.
 */
export async function apiFetch<T>(path: string, options: RequestInit = {}): Promise<T> {
  const baseUrl = import.meta.env.VITE_API_URL
  let accessToken = getAccessToken()

  const makeRequest = async (token: string | null) =>
    fetch(`${baseUrl}${path}`, {
      ...options,
      headers: {
        "Content-Type": "application/json",
        Authorization: token ? `Bearer ${token}` : "",
        ...options.headers,
      },
    })

  let res = await makeRequest(accessToken)

  // Если accessToken недействителен
  if (res.status === 401) {
    try {
      accessToken = await refreshAccessToken()
      res = await makeRequest(accessToken)
    } catch {
      clearAuth()
      window.location.href = "/login"
      throw new Error("Требуется авторизация")
    }
  }

  if (!res.ok) {
    let body: any
    try {
      body = await res.json()
    } catch {
      throw new Error(`Ошибка ${res.status}`)
    }

    // RFC7807-ошибка
    if (body?.type && body?.title && body?.status) {
      throw new ApiException(body as ApiError)
    }

    throw new Error(body.message ?? body.detail ?? `Ошибка ${res.status}`)
  }

  // 204 No Content
  if (res.status === 204) {
    return {} as T
  }

  return res.json()
}

/**
 * Обновление access токена через refresh токен
 */
async function refreshAccessToken(): Promise<string> {
  const refreshToken = getRefreshToken()
  if (!refreshToken) 
    throw new Error("Нет refresh-токена")

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
