import { useRef, useEffect } from "react"
import { useDebouncedCallback } from "@/shared/hooks/useDebouncedCallback"

export function useResizeObserver(
  onResize: (size: { width: number; height: number }) => void,
  delay = 50
) {
  const ref = useRef<HTMLDivElement | null>(null)

  const debouncedResize = useDebouncedCallback(onResize, delay)

  useEffect(() => {
    if (!ref.current) return

    const observer = new ResizeObserver(([entry]) => {
      const { width, height } = entry.contentRect
      debouncedResize({
        width: Math.floor(width),
        height: Math.floor(height),
      })
    })

    observer.observe(ref.current)
    return () => observer.disconnect()
  }, [debouncedResize])

  return ref
}