import { Card, CardHeader, CardTitle, CardContent, CardFooter, CardDescription } from "@/components/ui/card"
import { Button } from "@/components/ui/button"
import { SwordsIcon } from "lucide-react"
import { Game } from "@/features/game/Game"
import { useArenaStore } from "@/features/game/state/data/arena.data.store"
import { Skeleton } from "@/components/ui/skeleton"

export function ArenaCard() {
  const arena = useArenaStore((s) => s.arena)
  if (!arena) {
    return <Skeleton className="w-full h-full rounded-none md:rounded-r-2xl" />
  }

  return (
    <Card className="h-full rounded-none md:rounded-r-2xl flex flex-col">
      <CardHeader>
        <CardTitle>{arena.name}</CardTitle>
        <CardDescription>{arena.description}</CardDescription>
      </CardHeader>
      <CardContent className="flex-1 p-0 flex justify-center items-center">
        <Game />
      </CardContent>
      <CardFooter className="flex justify-end">
        <Button size="lg"><SwordsIcon /> В бой</Button>
      </CardFooter>
    </Card>
  )
}
