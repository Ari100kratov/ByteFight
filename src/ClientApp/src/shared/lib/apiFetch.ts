import { apiUrl } from "@/shared/config/api"
import { clearAuth, getAccessToken, getRefreshToken, saveAuthTokens } from "./auth"

export interface ApiErrorItem {
  code?: string
  description?: string
  type?: number
}

/**
 * Формат ошибки, возвращаемый API (RFC 7807 / CustomResults.Problem).
 */
export interface ApiError {
  type: string
  title: string
  status: number
  detail?: string
  errors?: Record<string, string[]> | ApiErrorItem[]
  traceId?: string
}

interface RefreshTokenResponse {
  accessToken: string
  refreshToken: string
}

/**
 * Ошибка, нормализованная из problem details ответа API.
 */
export class ApiException extends Error {
  public readonly status: number
  public readonly type: string
  public readonly title: string
  public readonly detail: string
  public readonly errors?: Record<string, string[]> | ApiErrorItem[]
  public readonly traceId?: string

  constructor(problem: ApiError) {
    super(problem.detail ?? problem.title)
    this.name = "ApiException"
    this.status = problem.status
    this.type = problem.type
    this.title = problem.title
    this.detail = problem.detail ?? problem.title
    this.errors = problem.errors
    this.traceId = problem.traceId
  }
}

/**
 * Выполняет запрос к API, обновляет access token при 401 и нормализует ошибки.
 */
export async function apiFetch<T>(path: string, options: RequestInit = {}): Promise<T> {
  let accessToken = getAccessToken()

  const makeRequest = async (token: string | null) => {
    const headers = new Headers(options.headers)
    headers.set("Content-Type", "application/json")

    if (token) {
      headers.set("Authorization", `Bearer ${token}`)
    } else {
      headers.delete("Authorization")
    }

    return fetch(apiUrl(path), {
      ...options,
      headers,
    })
  }

  let res = await makeRequest(accessToken)

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
    let body: unknown
    try {
      body = await res.json()
    } catch {
      throw new Error(`Ошибка ${String(res.status)}`)
    }

    if (isApiError(body)) {
      throw new ApiException(body)
    }

    throw new Error(getResponseErrorMessage(body, res.status))
  }

  if (res.status === 204) {
    return {} as T
  }

  const data: unknown = await res.json()

  return data as T
}

async function refreshAccessToken(): Promise<string> {
  const refreshToken = getRefreshToken()
  if (!refreshToken) throw new Error("Нет refresh-токена")

  const res = await fetch(apiUrl("/users/refresh-token"), {
    method: "POST",
    headers: { "Content-Type": "application/json" },
    body: JSON.stringify({ refreshToken }),
  })

  if (!res.ok) {
    clearAuth()
    throw new Error("Не удалось обновить токен")
  }

  const json: unknown = await res.json()

  if (!isRefreshTokenResponse(json)) {
    clearAuth()
    throw new Error("Некорректный ответ обновления токена")
  }

  saveAuthTokens(json.accessToken, json.refreshToken)

  return json.accessToken
}

function getResponseErrorMessage(body: unknown, status: number) {
  if (isRecord(body)) {
    const message = getOptionalString(body, "message") ?? getOptionalString(body, "detail")

    if (message) {
      return message
    }
  }

  return `Ошибка ${String(status)}`
}

function isApiError(value: unknown): value is ApiError {
  return (
    isRecord(value) &&
    typeof value.type === "string" &&
    typeof value.title === "string" &&
    typeof value.status === "number" &&
    isOptionalString(value.detail) &&
    isApiErrors(value.errors) &&
    isOptionalString(value.traceId)
  )
}

function isRefreshTokenResponse(value: unknown): value is RefreshTokenResponse {
  return (
    isRecord(value) &&
    typeof value.accessToken === "string" &&
    typeof value.refreshToken === "string"
  )
}

function isApiErrors(value: unknown): value is ApiError["errors"] {
  if (value === undefined) {
    return true
  }

  if (Array.isArray(value)) {
    return value.every(isApiErrorItem)
  }

  return isRecord(value) && Object.values(value).every(isStringArray)
}

function isApiErrorItem(value: unknown): value is ApiErrorItem {
  return (
    isRecord(value) &&
    isOptionalString(value.code) &&
    isOptionalString(value.description) &&
    isOptionalNumber(value.type)
  )
}

function getOptionalString(record: Record<string, unknown>, key: string) {
  const value = record[key]

  return typeof value === "string" ? value : undefined
}

function isRecord(value: unknown): value is Record<string, unknown> {
  return typeof value === "object" && value !== null
}

function isStringArray(value: unknown): value is string[] {
  return Array.isArray(value) && value.every((item) => typeof item === "string")
}

function isOptionalString(value: unknown): value is string | undefined {
  return value === undefined || typeof value === "string"
}

function isOptionalNumber(value: unknown): value is number | undefined {
  return value === undefined || typeof value === "number"
}
