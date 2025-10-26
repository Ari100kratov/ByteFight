import { Alert, AlertDescription, AlertTitle } from "@/components/ui/alert"
import type { ApiError } from "@/shared/lib/apiFetch"
import { AlertCircleIcon } from "lucide-react"

type ErrorAlertProps = {
  error: unknown
}

export function ErrorAlert({ error }: ErrorAlertProps) {
  let message = "Неизвестная ошибка"
  let details: string | undefined

  if (error instanceof Error) {
    message = error.message
    details = error.stack
  } else if (typeof error === "string") {
    message = error
  } else if (typeof error === "object" && error != null) {
    try {
      message = JSON.stringify(error, null, 2)
    } catch {
      message = "Ошибка сериализации"
    }
  }

  const apiError = (error as ApiError)?.detail
    ? (error as ApiError)
    : undefined

  return (
    <>
      <Alert variant="destructive">
        <AlertCircleIcon />
        <AlertTitle>
          {apiError?.title ?? "Что-то пошло не так..."}
        </AlertTitle>
        <AlertDescription>
          {apiError?.detail ?? message}
          {apiError?.status && (
            <div className="mt-1 text-xs text-muted-foreground">
              Код: {apiError.status}
            </div>
          )}
        </AlertDescription>
      </Alert>

      {process.env.NODE_ENV === "development" && details && (
        <pre className="w-full max-h-64 overflow-auto rounded bg-muted p-2 text-xs text-muted-foreground whitespace-pre-wrap">
          {details}
        </pre>
      )}
    </>
  )
}
