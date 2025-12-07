import { useEffect } from "react"
import { useBreadcrumbNames } from "@/layouts/BreadcrumbProvider"

export const ModeNames: Record<string, string> = {
  training: "Тренировка",
  pve: "PvE",
  pvp: "PvP",
}

export function useArenaBreadcrumbs(params: {
  modeType?: string
  arena?: { id: string; name: string }
}) {
  const { modeType, arena } = params
  const { setName } = useBreadcrumbNames()

  useEffect(() => {
    if (!modeType) return
    setName(`/play/${modeType}`, ModeNames[modeType] ?? "Неизвестный режим")
  }, [modeType])

  useEffect(() => {
    if (!arena || !modeType) return
    setName(`/play/${modeType}/${arena.id}`, arena.name)
  }, [arena, modeType])
}
