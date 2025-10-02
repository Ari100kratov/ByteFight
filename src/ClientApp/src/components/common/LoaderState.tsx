import { Skeleton } from "@/components/ui/skeleton"
import { Alert, AlertDescription, AlertTitle } from "@/components/ui/alert"
import { type ReactNode } from "react"
import { AlertCircleIcon } from "lucide-react"

type LoaderStateProps = {
  isLoading: boolean
  error: unknown
  children: ReactNode
  /** Что показать если children пустой */
  empty?: ReactNode
  /** Кастомный контент для загрузки */
  loadingFallback?: ReactNode
  /** CSS классы для дефолтного скелетона */
  skeletonClassName?: string
}

export function LoaderState({
  isLoading,
  error,
  children,
  empty = <div className="text-muted-foreground">Нет данных</div>,
  loadingFallback,
  skeletonClassName = "w-full h-64 rounded-md",
}: LoaderStateProps) {
  if (isLoading) {
    return loadingFallback ?? <Skeleton className={skeletonClassName} />
  }

  if (error) {
    const message =
      error instanceof Error
        ? error.message
        : typeof error === "string"
          ? error
          : "Неизвестная ошибка"

    const details =
      error instanceof Error
        ? error.stack
        : JSON.stringify(error, null, 2)

    return (
      <>
        <Alert variant="destructive">
          <AlertCircleIcon />
          <AlertTitle>Что-то пошло не так...</AlertTitle>
          <AlertDescription>{message}</AlertDescription>
        </Alert>

        {process.env.NODE_ENV === "development" && details && (
          <pre className="w-full max-h-64 overflow-auto rounded bg-muted p-2 text-xs text-muted-foreground whitespace-pre-wrap">
            {details}
          </pre>
        )}
      </>
    )
  }

  const isEmpty =
    !children ||
    (Array.isArray(children) && children.length === 0)

  return <>{isEmpty ? empty : children}</>
}
