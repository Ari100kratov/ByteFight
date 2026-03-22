import { Card, CardHeader, CardTitle, CardContent } from "@/components/ui/card"
import { ScrollArea } from "@/components/ui/scroll-area"
import { CombatLogTurnGroup } from "./CombatLogTurnGroup"
import { useGameRuntimeStore } from "../state/game.runtime.store"
import { useAutoScrollToBottom } from "./hooks/useAutoScrollToBottom"

export function CombatLogPanel() {
  const turnLogs = useGameRuntimeStore(s => s.turnLogs)
  const logEntriesCount = turnLogs.reduce((sum, turn) => sum + turn.logs.length, 0)

  const { viewportRef, bottomRef } = useAutoScrollToBottom({
    trigger: logEntriesCount,
  })

  return (
    <Card className="flex h-full flex-col">
      <CardHeader>
        <CardTitle>Журнал боя</CardTitle>
      </CardHeader>

      <CardContent className="flex-1 overflow-hidden">
        <ScrollArea className="h-full pr-4" viewportRef={viewportRef}>
          {turnLogs.map(turn => (
            <CombatLogTurnGroup
              key={turn.turnIndex}
              turn={turn}
            />
          ))}

          <div ref={bottomRef} />
        </ScrollArea>
      </CardContent>
    </Card>
  )
}