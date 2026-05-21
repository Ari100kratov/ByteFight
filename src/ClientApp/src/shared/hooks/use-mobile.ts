import * as React from "react"

const MOBILE_BREAKPOINT = 768
const MOBILE_MEDIA_QUERY = `(max-width: ${String(MOBILE_BREAKPOINT - 1)}px)`

function subscribeToMobileQuery(onStoreChange: () => void) {
  const mediaQueryList = window.matchMedia(MOBILE_MEDIA_QUERY)
  mediaQueryList.addEventListener("change", onStoreChange)

  return () => {
    mediaQueryList.removeEventListener("change", onStoreChange)
  }
}

function getMobileSnapshot() {
  return window.matchMedia(MOBILE_MEDIA_QUERY).matches
}

function getServerMobileSnapshot() {
  return false
}

export function useIsMobile() {
  return React.useSyncExternalStore(
    subscribeToMobileQuery,
    getMobileSnapshot,
    getServerMobileSnapshot,
  )
}
