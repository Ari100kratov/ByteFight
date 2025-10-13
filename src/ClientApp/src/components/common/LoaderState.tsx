import { Skeleton } from "@/components/ui/skeleton"
import { type ReactNode } from "react"
import { ErrorAlert } from "./ErrorAlert"

type LoaderStateProps = {
  isLoading: boolean
  error: unknown
  children: ReactNode
  empty?: ReactNode
  loadingFallback?: ReactNode
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
    return <ErrorAlert error={error} />
  }

  const isEmpty =
    !children ||
    (Array.isArray(children) && children.length === 0)

  return <>{isEmpty ? empty : children}</>
}
