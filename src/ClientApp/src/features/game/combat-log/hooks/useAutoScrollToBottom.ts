import { useEffect, useRef } from "react"

type UseAutoScrollToBottomParams = {
  trigger: number
  threshold?: number
  behavior?: ScrollBehavior
}

export function useAutoScrollToBottom({
  trigger,
  threshold = 40,
  behavior = "smooth",
}: UseAutoScrollToBottomParams) {
  const viewportRef = useRef<HTMLDivElement | null>(null)
  const bottomRef = useRef<HTMLDivElement | null>(null)
  const shouldAutoScrollRef = useRef(true)

  useEffect(() => {
    const viewport = viewportRef.current
    if (!viewport) return

    const updateAutoScrollState = () => {
      const distanceToBottom =
        viewport.scrollHeight - viewport.scrollTop - viewport.clientHeight

      shouldAutoScrollRef.current = distanceToBottom <= threshold
    }

    updateAutoScrollState()
    viewport.addEventListener("scroll", updateAutoScrollState)

    return () => {
      viewport.removeEventListener("scroll", updateAutoScrollState)
    }
  }, [threshold])

  useEffect(() => {
    if (!shouldAutoScrollRef.current) {
      return
    }

    bottomRef.current?.scrollIntoView({ behavior, block: "end" })
  }, [trigger, behavior])

  return {
    viewportRef,
    bottomRef,
    scrollToBottom: () => {
      bottomRef.current?.scrollIntoView({ behavior, block: "end" })
    },
  }
}