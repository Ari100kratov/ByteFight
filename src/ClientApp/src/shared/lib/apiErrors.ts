import { ApiException, type ApiErrorItem } from "./apiFetch"

export function getApiErrorMessages(error: unknown): string[] {
  if (!error) return []

  if (error instanceof ApiException) {
    if (Array.isArray(error.errors)) {
      return error.errors
        .map((item: ApiErrorItem) => item.description)
        .filter((description): description is string => Boolean(description))
    }

    if (error.errors) {
      return Object.values(error.errors).flat()
    }

    if (error.detail) return [error.detail]
    if (error.title) return [error.title]

    return [error.message]
  }

  if (error instanceof Error) {
    return [error.message]
  }

  return [String(error)]
}

export function getApiErrorToastMessage(error: unknown) {
  return getApiErrorMessages(error)[0] ?? "Не удалось выполнить действие"
}