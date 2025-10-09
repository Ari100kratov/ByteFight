import { Card, CardHeader, CardTitle, CardContent, CardFooter, CardDescription } from "@/components/ui/card"
import { Button } from "@/components/ui/button"
import { SwordsIcon } from "lucide-react"
import type { Arena } from "../useArena"

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
      <CardContent className="flex-1 flex items-center justify-center bg-muted rounded-md text-muted-foreground">
        {`Размер: ${arena.gridWidth} x ${arena.gridHeight}`}
      </CardContent>
      <CardFooter className="flex justify-end">
        <Button size="lg"><SwordsIcon /> В бой</Button>
      </CardFooter>
    </Card>
  )
}
