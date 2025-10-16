import { Card, CardHeader, CardTitle, CardContent, CardFooter, CardDescription } from "@/components/ui/card"
import { Button } from "@/components/ui/button"
import { SwordsIcon } from "lucide-react"
import type { Arena } from "../useArena"
import { Game } from "@/features/game/Game"

type Props = {
  arena: Arena
}

export function ArenaCard({ arena }: Props) {

  return (
    <Card className="h-full rounded-none md:rounded-r-2xl flex flex-col">
      <CardHeader>
        <CardTitle>{arena.name}</CardTitle>
        <CardDescription>{arena.description}</CardDescription>
      </CardHeader>
      <CardContent className="flex-1 p-0 flex justify-center items-center">
        <Game arena={arena}/>
      </CardContent>
      <CardFooter className="flex justify-end">
        <Button size="lg"><SwordsIcon /> В бой</Button>
      </CardFooter>
    </Card>
  )
}
