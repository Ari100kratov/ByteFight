import { useEffect } from "react"

export function useConfirmPageLeave(shouldBlock: boolean) {
  useEffect(() => {
    if (!shouldBlock) return

    const handleBeforeUnload = (event: BeforeUnloadEvent) => {
      event.preventDefault()
    }

    window.addEventListener("beforeunload", handleBeforeUnload)

    return () => {
      window.removeEventListener("beforeunload", handleBeforeUnload)
    }
  }, [shouldBlock])
}