import { useRef, useEffect, useCallback } from "react"

/**
 * Возвращает функцию с дебаунсом.
 * @param callback функция, которую нужно вызвать
 * @param delay задержка в мс
 */
export function useDebouncedCallback<T extends (...args: any[]) => void>(
  callback: T,
  delay: number
) {
  const timeoutRef = useRef<number | null>(null)

  const debouncedFn = useCallback(
    (...args: Parameters<T>) => {
      if (timeoutRef.current) clearTimeout(timeoutRef.current)
      timeoutRef.current = window.setTimeout(() => {
        callback(...args)
      }, delay)
    },
    [callback, delay]
  )

  useEffect(() => {
    return () => {
      if (timeoutRef.current) clearTimeout(timeoutRef.current)
    }
  }, [])

  return debouncedFn
}
