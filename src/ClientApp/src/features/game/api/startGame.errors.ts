import { ApiException } from "@/shared/lib/apiFetch"

export type StartGameUiError = {
  title: string
  detail: string
  code?: string
}

export function mapStartGameError(error: unknown): StartGameUiError {
  if (error instanceof ApiException) {
    switch (error.title) {
      case "GameHost.AlreadyRunningForUser":
        return {
          title: "Бой уже запущен",
          detail: error.detail || "У вас уже есть активная игровая сессия. Завершите текущий бой перед запуском нового.",
          code: error.title,
        }

      case "GameHost.UserCodeCompilationFailed":
        return {
          title: "Ошибка в пользовательском коде",
          detail: error.detail || "Не удалось скомпилировать пользовательский код.",
          code: error.title,
        }

      case "GameSession.InvalidMode":
        return {
          title: "Некорректный режим игры",
          detail: error.detail || "Указан неизвестный режим игры.",
          code: error.title,
        }

      case "GameSession.CodeIsRequired":
        return {
          title: "Код не задан",
          detail: error.detail || "Для запуска боя необходимо указать пользовательский код.",
          code: error.title,
        }

      default:
        return {
          title: "Не удалось начать бой",
          detail: error.detail || "Во время запуска игровой сессии произошла ошибка.",
          code: error.title,
        }
    }
  }

  if (error instanceof Error) {
    return {
      title: "Не удалось начать бой",
      detail: error.message,
    }
  }

  return {
    title: "Не удалось начать бой",
    detail: "Во время запуска игровой сессии произошла неизвестная ошибка.",
  }
}