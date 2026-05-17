import { Alert, AlertDescription, AlertTitle } from "@/components/ui/alert"
import { ApiException } from "@/shared/lib/apiFetch"
import { AlertCircleIcon } from "lucide-react"

interface ErrorAlertProps {
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

  const apiError = error instanceof ApiException ? error : undefined

  return (
    <>
      <Alert variant="destructive">
        <AlertCircleIcon />
        <AlertTitle>{apiError?.title ?? "Что-то пошло не так..."}</AlertTitle>
        <AlertDescription>
          {apiError?.detail ?? message}
          {apiError?.status && (
            <div className="text-muted-foreground mt-1 text-xs">Код: {apiError.status}</div>
          )}
        </AlertDescription>
      </Alert>

      {import.meta.env.DEV && details && (
        <pre className="bg-muted text-muted-foreground max-h-64 w-full overflow-auto rounded p-2 text-xs whitespace-pre-wrap">
          {details}
        </pre>
      )}
    </>
  )
}
