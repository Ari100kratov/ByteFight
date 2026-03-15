import { Card, CardHeader, CardTitle, CardContent } from "@/components/ui/card"
import { ScrollArea } from "@/components/ui/scroll-area"
import { CombatLogTurnGroup } from "./CombatLogTurnGroup"
import { useGameRuntimeStore } from "../state/game.runtime.store"

export function CombatLogPanel() {
  const turnLogs = useGameRuntimeStore(s => s.turnLogs)

  return (
    <Card className="flex flex-col h-full">
      <CardHeader>
        <CardTitle>Журнал боя</CardTitle>
      </CardHeader>

      <CardContent className="flex-1 overflow-hidden">
        <ScrollArea className="h-full pr-4">
          {turnLogs.map(turn => (
            <CombatLogTurnGroup
              key={turn.turnIndex}
              turn={turn}
            />
          ))}
        </ScrollArea>
      </CardContent>
    </Card>
  )
}