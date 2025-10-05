import { useEffect, useMemo } from "react"
import { useParams } from "react-router-dom"
import { useBreadcrumbNames } from "@/layouts/BreadcrumbProvider"

export default function GameMapsPage() {
  const { id } = useParams<{ id: string }>()
  const { setName } = useBreadcrumbNames()

  const modeName = useMemo(() => {
    switch (id) {
      case "training":
        return "Тренировка"
      case "pve":
        return "PvE"
      case "pvp":
        return "PvP"
      default:
        return "Неизвестный режим"
    }
  }, [id])

  useEffect(() => {
    if (id) {
      setName(`/play/${id}`, modeName)
    }
  }, [id, modeName, setName])

  return (
    <div className="p-6 flex flex-col gap-6">
      <h1 className="text-3xl font-bold">{modeName}</h1>
      <p className="text-muted-foreground">
        Здесь будут отображаться игровые арены и карты выбранного режима.
      </p>

      <div className="flex items-center justify-center h-[400px] border rounded-2xl text-muted-foreground">
        Заглушка для режима <span className="ml-1 font-semibold">{modeName}</span>
      </div>
    </div>
  )
}
