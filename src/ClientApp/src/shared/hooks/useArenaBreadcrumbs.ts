import { useEffect } from "react"
import { useBreadcrumbNames } from "@/layouts/BreadcrumbProvider"
import { formatModeNameByString } from "../types/modeNames"

export function useArenaBreadcrumbs(params: {
  modeType?: string
  arena?: { id: string; name: string }
}) {
  const { modeType, arena } = params
  const { setName } = useBreadcrumbNames()

  useEffect(() => {
    if (!modeType) return
    setName(`/play/${modeType}`, formatModeNameByString(modeType))
  }, [modeType])

  useEffect(() => {
    if (!arena || !modeType) return
    setName(`/play/${modeType}/${arena.id}`, arena.name)
  }, [arena, modeType])
}
