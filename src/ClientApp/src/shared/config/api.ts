const apiBaseUrl = normalizeBaseUrl(import.meta.env.VITE_API_URL ?? "")
const configuredGameHubUrl = normalizeBaseUrl(import.meta.env.VITE_GAME_HUB_URL ?? "")

export function apiUrl(path: string): string {
  return joinUrl(apiBaseUrl, path)
}

export function gameHubUrl(): string {
  if (configuredGameHubUrl) {
    return configuredGameHubUrl
  }

  if (isAbsoluteUrl(apiBaseUrl)) {
    return joinUrl(apiBaseUrl, "/game-runtime-hub")
  }

  return "/game-runtime-hub"
}

function normalizeBaseUrl(value: string): string {
  return value.trim().replace(/\/+$/, "")
}

function joinUrl(baseUrl: string, path: string): string {
  const normalizedPath = path.startsWith("/") ? path : `/${path}`
  return `${baseUrl}${normalizedPath}`
}

function isAbsoluteUrl(value: string): boolean {
  return /^https?:\/\//i.test(value)
}
